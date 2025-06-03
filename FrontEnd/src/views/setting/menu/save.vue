<template>
	<el-row :gutter="40">
		<el-col v-if="!form.id">
			<el-empty description="请选择左侧菜单后操作" :image-size="100"></el-empty>
		</el-col>
		<template v-else>
			<el-col :lg="12">
				<h2>{{ form.meta.title || "新增菜单" }}</h2>
				<el-form :model="form" :rules="rules" ref="dialogForm" label-width="80px" label-position="left">
					<el-form-item label="显示名称" prop="meta.title">
						<el-input v-model="form.meta.title" clearable placeholder="菜单显示名字"></el-input>
					</el-form-item>
					<el-form-item label="上级菜单" prop="parentId">
						<el-cascader v-model="form.parentId" :options="menuOptions" :props="menuProps" :show-all-levels="false"
							placeholder="顶级菜单" clearable disabled></el-cascader>
					</el-form-item>
					<el-form-item label="类型" prop="meta.type">
						<el-radio-group v-model="form.meta.type">
							<el-radio-button label="menu">菜单</el-radio-button>
							<el-radio-button label="iframe">Iframe</el-radio-button>
							<el-radio-button label="link">外链</el-radio-button>
							<el-radio-button label="button">按钮</el-radio-button>
						</el-radio-group>
					</el-form-item>
					<el-form-item label="别名" prop="name">
						<el-input v-model="form.name" clearable placeholder="菜单别名"></el-input>
						<div class="el-form-item-msg">系统唯一且与内置组件名一致，否则导致缓存失效。如类型为Iframe的菜单，别名将代替源地址显示在地址栏</div>
					</el-form-item>
					<el-form-item label="菜单图标" prop="meta.icon">
						<sc-icon-select v-model="form.meta.icon" clearable></sc-icon-select>
					</el-form-item>
					<el-form-item label="路由地址" prop="path">
						<el-input v-model="form.path" clearable placeholder="例如/menu"></el-input>
					</el-form-item>
					<el-form-item label="重定向" prop="redirect">
						<el-input v-model="form.redirect" clearable placeholder=""></el-input>
					</el-form-item>
					<el-form-item label="菜单高亮" prop="menuActive">
						<el-input v-model="form.menuActive" clearable placeholder=""></el-input>
						<div class="el-form-item-msg">子节点或详情页需要高亮的上级菜单路由地址</div>
					</el-form-item>
					<el-form-item label="视图" prop="component">
						<el-input v-model="form.component" clearable placeholder="">
							<template #prepend>views/</template>
						</el-input>
						<div class="el-form-item-msg">如父节点、链接或Iframe等没有视图的菜单不需要填写</div>
					</el-form-item>
					<el-form-item label="颜色" prop="color">
						<el-color-picker v-model="form.meta.color" :predefine="predefineColors"></el-color-picker>

					</el-form-item>
					<el-form-item label="是否隐藏" prop="meta.hidden">
						<el-checkbox v-model="form.meta.hidden">隐藏菜单</el-checkbox>
						<el-checkbox v-model="form.meta.hiddenBreadcrumb">隐藏面包屑</el-checkbox>
						<div class="el-form-item-msg">菜单不显示在导航中，但用户依然可以访问，例如详情页</div>
					</el-form-item>
					<el-form-item label="整页路由" prop="fullpage">
						<el-switch v-model="form.meta.fullpage" />
					</el-form-item>
					<el-form-item label="标签" prop="tag">
						<el-input v-model="form.meta.tag" clearable placeholder=""></el-input>
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="save" :loading="loading">保 存</el-button>
					</el-form-item>
				</el-form>

			</el-col>
			<el-col :lg="12" class="apilist">
				<h2>接口权限</h2>
				<sc-form-table v-model="form.apiList" :addTemplate="apiListAddTemplate" placeholder="暂无匹配接口权限">

					<el-table-column prop="url" label="Api url" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.url" placeholder="请选择" popper-class="api_select" clearable filterable>
								<el-option v-for="api in apiList" :key="api.url" :label="api.url" :value="api.url"
									style="min-height: 40px;">
									<!-- <span style="float: left">{{ api.url }}</span>
									<span style="float: right; color: #8492a6; font-size: 13px">{{ api.summary }}</span> -->
									<template #default>
										<div>{{ api.url }}</div>
										<div style="font-size: 12px; color: #999;" v-if="api.summary && api.summary != ''">{{ api.summary }}
										</div>

									</template>
								</el-option>
							</el-select>
						</template>
					</el-table-column>
				</sc-form-table>
			</el-col>
		</template>
	</el-row>
</template>

<script>
import scIconSelect from '@/components/scIconSelect'

export default {
	components: {
		scIconSelect
	},
	props: {
		menu: { type: Object, default: () => { } },
	},
	data() {
		return {
			apiList: [],
			form: {
				id: "",
				parentId: "",
				name: "",
				path: "",
				component: "",
				redirect: "",
				meta: {
					title: "",
					icon: "",
					menuActive: "",
					color: "",
					type: "menu",
					fullpage: false,
					tag: "",
				},
				apiList: []
			},
			menuOptions: [],
			menuProps: {
				value: 'id',
				label: 'title',
				checkStrictly: true
			},
			predefineColors: [
				'#ff4500',
				'#ff8c00',
				'#ffd700',
				'#67C23A',
				'#00ced1',
				'#409EFF',
				'#c71585'
			],
			rules: [],
			apiListAddTemplate: {
				url: ""
			},
			loading: false
		}
	},
	watch: {
		menu: {
			handler() {
				this.menuOptions = this.treeToMap(this.menu)
			},
			deep: true
		}
	},
	async mounted() {
		var res = await this.$API.system.menu.getApiList.get();
		if (res.code == "20000") {
			this.apiList = res.data
		}
	},
	methods: {
		//简单化菜单
		treeToMap(tree) {
			const map = []
			tree.forEach(item => {
				var obj = {
					id: item.id,
					parentId: item.parentId,
					title: item.meta.title,
					children: item.children && item.children.length > 0 ? this.treeToMap(item.children) : null
				}
				map.push(obj)
			})
			return map
		},
		//保存
		async save() {
			this.loading = true
			var res = await this.$API.system.menu.updateMenu.post(this.form)
			this.loading = false
			if (res.code == 20000) {
				this.$message.success("保存成功")
			} else {
				this.$message.warning(res.msg)
			}
		},
		//表单注入数据
		setData(data) {

			this.form = data
			this.form.apiList = data.apiList || []
			// this.form.parentId = pid
		}
	}
}
</script>

<style>
.api_select {
	height: unset !important;

	.el-select-dropdown__item {
		height: unset;
		line-height: unset;
		border-bottom: 1px solid #dcdfe6;
		padding: 5px 30px 5px;
	}
}
</style>

<style scoped>
h2 {
	font-size: 17px;
	color: #3c4a54;
	padding: 0 0 30px 0;
}

.apilist {
	border-left: 1px solid #eee;
}

[data-theme="dark"] h2 {
	color: #fff;
}

[data-theme="dark"] .apilist {
	border-color: #434343;
}
</style>
