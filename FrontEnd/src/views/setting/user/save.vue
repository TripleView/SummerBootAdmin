<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode == 'show'" ref="dialogForm" label-width="100px"
			label-position="left">
			<!-- <el-form-item label="头像" prop="avatar">
				<sc-upload v-model="form.avatar" title="上传头像"></sc-upload>
			</el-form-item> -->
			<el-form-item label="登录账号" prop="account">
				<el-input v-model="form.account" placeholder="用于登录系统" clearable></el-input>
			</el-form-item>
			<el-form-item label="姓名" prop="name">
				<el-input v-model="form.name" placeholder="请输入完整的真实姓名" clearable></el-input>
			</el-form-item>
			<template v-if="mode == 'add'">
				<el-form-item label="登录密码" prop="password">
					<el-input type="password" v-model="form.password" clearable show-password></el-input>
				</el-form-item>
				<el-form-item label="确认密码" prop="password2">
					<el-input type="password" v-model="form.password2" clearable show-password></el-input>
				</el-form-item>
			</template>

			<el-form-item label="所属部门" prop="departmentId">
				<treeSelect v-model="form.departmentId" :multiple="false" :options="depts" :normalizer="normalizer" />
				<!-- <treeselect v-model="value" /> -->
				<!-- <el-cascader v-model="form.departmentId" :options="depts" :props="deptsProps" clearable
					style="width: 100%;"></el-cascader> -->
			</el-form-item>
			<el-form-item label="所属角色" prop="roleIds">
				<el-select v-model="form.roleIds" filterable style="width: 100%" multiple>
					<el-option v-for="item in groups" :key="item.id" :label="item.name" :value="item.id" />
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

			mode: "add",
			titleMap: {
				add: '新增用户',
				edit: '编辑用户',
				show: '查看'
			},
			visible: false,
			isSaveing: false,
			//表单数据
			form: {
				id: "",
				userName: "",
				password: "",
				avatar: "",
				name: "",
				departmentId: "",
				roleIds: []
			},
			//验证规则
			rules: {
				avatar: [
					{ required: true, message: '请上传头像' }
				],
				account: [
					{ required: true, message: '请输入登录账号' }
				],
				name: [
					{ required: true, message: '请输入真实姓名' }
				],
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
				departmentId: [
					{ required: true, message: '请选择所属部门' }
				],
				roleIds: [
					{ required: true, message: '请选择所属角色', trigger: 'change' }
				]
			},
			//所需数据选项
			groups: [],
			groupsProps: {
				value: "id",
				label: "name",
				multiple: true,
				checkStrictly: true
			},
			depts: [],
			deptsProps: {
				value: "id",
				label: "name",
				checkStrictly: true
			}
		}
	},
	mounted() {
		this.getRole()
		this.getDept()
	},
	methods: {
		normalizer(node) {
			return {
				id: node.id,
				label: node.name,
				children: node.children && node.children.length > 0 ? node.children : 0,
			}
		},
		//显示
		open(mode = 'add') {
			this.mode = mode;
			this.visible = true;
			return this
		},
		//加载树数据
		async getRole() {
			var res = await this.$API.role.list.get();
			console.log("角色", res)
			this.groups = res.data;
		},
		async getDept() {
			var res = await this.$API.department.tree.get();
			this.depts = res.data;
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
					if (this.mode == 'add') {
						res = await this.$API.user.addUser.post(this.form);
					} else {
						res = await this.$API.user.updateUser.post(this.form);
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
			this.form = this.$clone(data)

			//可以和上面一样单个注入，也可以像下面一样直接合并进去
			//Object.assign(this.form, data)
		}
	}
}
</script>

<style></style>
