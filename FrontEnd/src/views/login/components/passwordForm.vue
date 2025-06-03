<template>
	<el-form ref="loginForm" :model="form" :rules="rules" label-width="0" size="large" @keyup.enter="login">
		<el-form-item prop="user">
			<el-input v-model="form.user" prefix-icon="el-icon-user" clearable :placeholder="$t('login.userPlaceholder')">
			</el-input>
		</el-form-item>
		<el-form-item prop="password">
			<el-input v-model="form.password" prefix-icon="el-icon-lock" clearable show-password
				:placeholder="$t('login.PWPlaceholder')"></el-input>
		</el-form-item>

		<el-form-item>
			<el-button type="primary" style="width: 100%;" :loading="islogin" round @click="login">{{ $t('login.signIn')
			}}</el-button>
		</el-form-item>

	</el-form>
</template>

<script>
export default {
	data() {
		return {
			userType: 'admin',
			form: {
				user: "admin",
				password: "admin",
				autologin: false
			},
			rules: {
				user: [
					{ required: true, message: this.$t('login.userError'), trigger: 'blur' }
				],
				password: [
					{ required: true, message: this.$t('login.PWError'), trigger: 'blur' }
				]
			},
			islogin: false,
		}
	},
	watch: {
		userType(val) {
			if (val == 'admin') {
				this.form.user = 'admin'
				this.form.password = 'admin'
			} else if (val == 'user') {
				this.form.user = 'user'
				this.form.password = 'user'
			}
		}
	},
	mounted() {

	},
	methods: {
		async login() {

			var validate = await this.$refs.loginForm.validate().catch(() => { })
			if (!validate) { return false }

			this.islogin = true
			var data = {
				account: this.form.user,
				password: this.form.password
			}
			//获取token
			var user = await this.$API.auth.token.post(data)
			if (user.code == 20000) {
				console.log("进来了")
				this.$TOOL.cookie.set("TOKEN", user.data.token, {
					expires: this.form.autologin ? 24 * 60 * 60 : 0
				})
				this.$TOOL.data.set("USER_INFO", user.data.userInfo)
			} else {
				this.islogin = false
				this.$message.warning(user.message)
				return false
			}
			//获取菜单
			var menu = await this.$API.system.menu.myMenus.get()

			if (menu.code == 20000) {
				if (menu.data.menu.length == 0) {
					this.islogin = false
					this.$alert("当前用户无任何菜单权限，请联系系统管理员", "无权限访问", {
						type: 'error',
						center: true
					})
					return false
				}
				this.$TOOL.data.set("MENU", menu.data.menu)
				this.$TOOL.data.set("PERMISSIONS", menu.data.permissions)
				this.$TOOL.data.set("DASHBOARDGRID", menu.data.dashboardGrid)
			} else {
				this.islogin = false
				this.$message.warning(menu.message)
				return false
			}

			this.$router.replace({
				path: '/'
			})
			this.$message.success("Login Success 登录成功")
			this.islogin = false
		},
	}
}
</script>

<style></style>
