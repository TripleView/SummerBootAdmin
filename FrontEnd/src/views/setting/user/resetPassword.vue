<template>
	<el-dialog title="重置密码" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode == 'show'" ref="dialogForm" label-width="100px"
			label-position="left">
			<el-form-item label="提示" prop="">
				<el-alert :title="tips" type="info" :closable="false" />
			</el-form-item>
			<el-form-item label="登录密码" prop="password">
				<el-input type="password" v-model="form.password" clearable show-password></el-input>
			</el-form-item>
			<el-form-item label="确认密码" prop="password2">
				<el-input type="password" v-model="form.password2" clearable show-password></el-input>
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
			tips: "",
			mode: "assignRole",

			visible: false,
			isSaveing: false,
			//表单数据
			form: {
				userIds: [],
				password: '',
				password2: '',
			},
			//验证规则
			rules: {
				password: [
					{ required: true, message: '请输入登录密码' },
					{
						validator: (rule, value, callback) => {
							if (this.form.password2 !== '') {
								this.$refs.dialogForm.validateField('password2');
							}
							callback();
						}
					}
				],
				password2: [
					{ required: true, message: '请再次输入密码' },
					{
						validator: (rule, value, callback) => {
							if (value !== this.form.password) {
								callback(new Error('两次输入密码不一致!'));
							} else {
								callback();
							}
						}
					}
				],
			},
		}
	},
	mounted() {

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
					res = await this.$API.user.resetPassword.post(this.form);

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
			if (data == null || data.length == 0) {
				return;
			}

			this.form.userIds = data.map(x => x.id)
			console.log("this.form", this.form)
			var names = data.map(x => x.name).join(";");
			this.tips = "为以下用户重置密码:" + names
			//可以和上面一样单个注入，也可以像下面一样直接合并进去
			//Object.assign(this.form, data)
		}
	}
}
</script>

<style></style>
