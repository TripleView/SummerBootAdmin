<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="400" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" ref="dialogForm" label-width="100px" label-position="left">
			<el-form-item label="所属字典" prop="dictionaryId">
				<el-cascader v-model="form.dictionaryId" :options="dic" :props="dicProps" :show-all-levels="false"
					clearable></el-cascader>
			</el-form-item>
			<el-form-item label="项名称" prop="name">
				<el-input v-model="form.name" clearable></el-input>
			</el-form-item>
			<el-form-item label="键值" prop="value">
				<el-input v-model="form.value" clearable></el-input>
			</el-form-item>
			<el-form-item label="排序" prop="index">
				<el-input type="number" v-model="form.index" clearable></el-input>
			</el-form-item>
			<!-- <el-form-item label="是否有效" prop="yx">
				<el-switch v-model="form.yx" active-value="1" inactive-value="0"></el-switch>
			</el-form-item> -->
		</el-form>
		<template #footer>
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
			mode: "add",
			titleMap: {
				add: '新增项',
				edit: '编辑项'
			},
			visible: false,
			isSaveing: false,
			form: {
				id: "",
				dictionaryId: "",
				name: "",
				key: "",
				index:""//排序
				// yx: "1"
			},
			rules: {
				dictionaryId: [
					{ required: true, message: '请选择所属字典' }
				],
				name: [
					{ required: true, message: '请输入项名称' }
				],
				value: [
					{ required: true, message: '请输入键值' }
				],
				index: [
					{ required: true, message: '请输入排序' }
				]
			},
			dic: [],
			dicProps: {
				value: "id",
				label: "name",
				emitPath: false,
				checkStrictly: true
			}
		}
	},
	mounted() {
		console.log("参数为", this.params)
		if (this.params) {

			this.form.dic = this.params.code
		}
		this.getDic()
	},
	methods: {
		//显示
		open(mode = 'add') {
			this.mode = mode;
			this.visible = true;
			return this;
		},
		//获取字典列表
		async getDic() {
			var res = await this.$API.system.dic.tree.get();
			this.dic = res.data;
		},
		//表单提交方法
		submit() {
			this.$refs.dialogForm.validate(async (valid) => {
				if (valid) {
					this.isSaveing = true;
					var res
					if (this.mode == 'add') {
						res = await this.$API.system.dic.addDicItem.post(this.form);
					} else {
						res = await this.$API.system.dic.updateDicItem.post(this.form);
					}

					this.isSaveing = false;
					if (res.code == 20000) {
						this.$emit('success', this.form, this.mode)
						this.visible = false;
						this.$message.success("操作成功")
					} else {
						this.$alert(res.msg, "提示", { type: 'error' })
					}
				}
			})
		},
		//表单注入数据
		setData(data) {
			this.form=this.$clone(data)
			// this.form.yx = data.yx
			this.form.dictionaryId = data.dictionaryId
		}
	}
}
</script>

<style></style>
