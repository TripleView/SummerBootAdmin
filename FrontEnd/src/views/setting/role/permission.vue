<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-tabs tab-position="top">
			<el-tab-pane label="菜单权限">
				<div class="treeMain">
					<el-tree ref="menu" node-key="id" :data="menu.list" :props="menu.props" show-checkbox></el-tree>
				</div>
			</el-tab-pane>
			<el-tab-pane label="数据权限">
				<el-form label-width="100px" label-position="left">
					<el-form-item label="规则类型">
						<el-select v-model="data.dataType" placeholder="请选择">
							<el-option label="全部可见" value="1"></el-option>
							<el-option label="本人可见" value="2"></el-option>
							<el-option label="所在部门可见" value="3"></el-option>
							<el-option label="所在部门及子级可见" value="4"></el-option>
							<el-option label="选择的部门可见" value="5"></el-option>
							<el-option label="自定义" value="6"></el-option>
						</el-select>
					</el-form-item>
					<!-- :props="data.props" -->
					<el-form-item label="选择部门" v-show="data.dataType == '5'">
						<div class="treeMain" style="width: 100%;">
							<el-tree ref="dept" node-key="id" :data="data.list" show-checkbox :props="departmentGroupsProps"></el-tree>
						</div>
					</el-form-item>
					<el-form-item label="规则值" v-show="data.dataType == '6'">
						<el-input v-model="data.rule" clearable type="textarea" :rows="6" placeholder="请输入自定义规则代码"></el-input>
					</el-form-item>
				</el-form>
			</el-tab-pane>
			<el-tab-pane label="控制台模块">
				<div class="treeMain">
					<el-tree ref="grid" node-key="key" :data="grid.list" :props="grid.props" :default-checked-keys="grid.checked"
						show-checkbox></el-tree>
				</div>
			</el-tab-pane>
			<el-tab-pane label="控制台">
				<el-form label-width="100px" label-position="left">
					<el-form-item label="控制台视图">
						<el-select v-model="dashboard" placeholder="请选择">
							<el-option v-for="item in dashboardOptions" :key="item.value" :label="item.label" :value="item.value">
								<span style="float: left">{{ item.label }}</span>
								<span style="float: right; color: #8492a6; font-size: 12px">{{ item.views }}</span>
							</el-option>
						</el-select>
						<div class="el-form-item-msg">用于控制角色登录后控制台的视图</div>
					</el-form-item>
				</el-form>
			</el-tab-pane>
		</el-tabs>
		<template #footer v-if="mode == 'assignRoles'">
			<el-button @click="visible = false">取 消</el-button>
			<el-button type="primary" :loading="isSaveing" @click="submit()">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>
export default {
	emits: ['success', 'closed'],
	data() {
		return {
			isSetData: false,
			roleIds: [],//角色id列表
			titleMap: {
				assignRoles: '分配角色权限',
				show: '查看角色权限'
			},
			mode: 'show',
			departmentGroupsProps: {
				value: "id",
				label: "name",
				emitPath: false,
				checkStrictly: true
			},
			visible: false,
			isSaveing: false,
			menu: {
				list: [],
				checked: [],
				props: {
					label: (data) => {
						return data.meta.title
					}
				}
			},
			grid: {
				list: [],
				checked: ["welcome", "ver", "time", "progress", "echarts", "about"],
				props: {
					label: (data) => {
						return data.title
					},
					disabled: (data) => {
						return data.isFixed
					}
				}
			},
			data: {
				dataType: "1",
				list: [],
				checked: [],
				props: {},
				rule: ""
			},
			dashboard: "0",
			dashboardOptions: [
				{
					value: '0',
					label: '数据统计',
					views: 'stats'

				},
				{
					value: '1',
					label: '工作台',
					views: 'work'
				},
			]
		}
	},
	mounted() {

		this.getDept()
		this.getGrid()
	},
	methods: {
		//表单注入数据
		setData(data) {
			if (data == null) {
				return;
			}

			if (Array.isArray(data)) {
				this.roleIds = data.map(x => x.id)
			} else {
				this.roleIds = [data.id]
			}
			this.isSetData = true;
			this.initData();
			// console.log("this.roleIds", this.roleIds)
			return this;
		},
		async initData() {
			if (this.isSetData && this.visible) {
				await this.getMenu()
				if (this.mode == 'show' && this.roleIds.length == 1) {
					var res = await this.$API.role.getRolePermissions.get(this.roleIds[0])
					this.$refs.menu.setCheckedKeys(res.data.menuIds)
				}
			}
		},
		open(mode = 'show') {
			this.mode = mode
			this.visible = true;
			this.initData();
			return this;
		},
		async submit() {
			this.isSaveing = true;

			//选中的和半选的合并后传值接口
			var menuIds = this.$refs.menu.getCheckedKeys()
			console.log(menuIds)

			var checkedKeys_dept = this.$refs.dept.getCheckedKeys()
			console.log(checkedKeys_dept)

			var postData = {};
			postData.menuIds = menuIds;
			postData.roleIds = this.roleIds;
			var res = await this.$API.role.roleAssignPermissions.post(postData);
			this.isSaveing = false;
			this.visible = false;

			if (res.code == '20000') {
				this.$message.success("操作成功")
				this.$emit('success')
			} else {
				this.$message.error(res.msg)
			}


		},
		async getMenu() {
			var res = await this.$API.system.menu.list.get()
			this.menu.list = res.data
		},
		async getDept() {
			var res = await this.$API.department.tree.get();
			this.data.list = res.data
			// this.data.checked = ["12", "2", "21", "22", "1"]
			// this.$nextTick(() => {
			// 	let filterKeys = this.data.checked.filter(key => this.$refs.dept.getNode(key).isLeaf)
			// 	this.$refs.dept.setCheckedKeys(filterKeys, true)
			// })
		},
		getGrid() {
			this.grid.list = [
				{
					key: "welcome",
					title: "欢迎",
					isFixed: true
				},
				{
					key: "ver",
					title: "版本信息",
					isFixed: true
				},
				{
					key: "time",
					title: "时钟"
				},
				{
					key: "progress",
					title: "进度环"
				},
				{
					key: "echarts",
					title: "实时收入"
				},
				{
					key: "about",
					title: "关于项目"
				}
			]
		}
	}
}
</script>

<style scoped>
.treeMain {
	height: 280px;
	overflow: auto;
	border: 1px solid #dcdfe6;
	margin-bottom: 10px;
}
</style>
