<template>
	<el-dialog title="分配角色" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode == 'show'" ref="dialogForm" label-width="100px"
			label-position="left">
			<el-form-item label="提示" prop="">
				<el-alert :title="tips" type="info" :closable="false" />
			</el-form-item>
			<el-form-item label="所属角色" prop="roleIds">
				<el-select v-model="form.roleIds" filterable style="width: 100%" multiple>
					<el-option v-for="item in roles" :key="item.id" :label="item.name" :value="item.id" />
				</el-select>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible = false">取 消</el-button>
			<el-button v-if="mode != 'show'" type="primary" :loading="isSaveing" @click="submit()">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>


export default {
	emits: ['success', 'closed'],
	data() {
		return {
			roles: [],//角色列表
			tips: "",
			mode: "assignRole",

			visible: false,
			isSaveing: false,
			//表单数据
			form: {
				userIds: [],
				roleIds: []
			},
			//验证规则
			rules: {

				roleIds: [
					{ required: true, message: '请选择所属角色', trigger: 'change' }
				]
			},

		}
	},
	mounted() {
		this.getRoles()
	},
	methods: {

		//显示
		open(mode = 'assignRole') {
			this.mode = mode;
			this.visible = true;
			return this
		},
		//表单提交方法
		submit() {
			this.$refs.dialogForm.validate(async (valid) => {
				if (valid) {
					this.isSaveing = true;

					// console.log("数据", this.form)
					// this.isSaveing = false;
					// return;
					var res
					res = await this.$API.user.assignRoles.post(this.form);

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
		//加载树数据
		async getRoles() {
			var res = await this.$API.role.list.get();
			this.roles = res.data;
		},
		//表单注入数据
		setData(data) {
			if (data == null || data.length == 0) {
				return;
			}

			this.form.userIds = data.map(x => x.id)
			var names = data.map(x => x.name).join(";");
			this.tips = "为以下用户分配角色:" + names
			//可以和上面一样单个注入，也可以像下面一样直接合并进去
			//Object.assign(this.form, data)
		}
	}
}
</script>

<style></style>
