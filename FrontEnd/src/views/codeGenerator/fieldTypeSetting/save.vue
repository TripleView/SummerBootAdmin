<template>
	<el-dialog
		:title="titleMap[mode]"
		v-model="visible"
		:width="600"
		destroy-on-close
		@closed="$emit('closed')"
	>
		<el-form
			:model="form"
			:rules="rules"
			:disabled="mode == 'show'"
			ref="dialogForm"
			label-width="200px"
		>
			<el-form-item label="字段类型名称" prop="name">
				<el-input
					v-model="form.name"
					placeholder="请输入字段类型名称"
					clearable
				></el-input>
			</el-form-item>

			<el-form-item label="字段类型" prop="value">
				<el-input
					v-model="form.value"
					placeholder="请输入字段类型"
					clearable
				></el-input>
			</el-form-item>
			<el-form-item label="描述" prop="description">
				<el-input
					v-model="form.description"
					placeholder="请输入描述"
					clearable
				></el-input>
			</el-form-item>
			<el-form-item label="排序" prop="description">
				<el-input-number v-model="form.orderIndex" />
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible = false">取 消</el-button>
			<el-button
				v-if="mode != 'show'"
				type="primary"
				:loading="isSaveing"
				@click="submit()"
				>保 存</el-button
			>
		</template>
	</el-dialog>
</template>

<script>
export default {
	emits: ["success", "closed"],
	data() {
		return {
			mode: "add",
			titleMap: {
				add: "新增",
				edit: "编辑",
				show: "查看",
			},
			visible: false,
			isSaveing: false,
			//表单数据
			form: {
				id: "",
				name: "",
				value: "",
				description: "",
				orderIndex: null,
			},
			//验证规则
			rules: {
				name: [{ required: true, message: "请输入字段类型名称" }],
				value: [{ required: true, message: "请输入字段类型" }],
			},
		};
	},
	mounted() {},
	methods: {
		//显示
		open(mode = "add") {
			this.mode = mode;
			this.visible = true;
			return this;
		},
		//表单提交方法
		async submit() {
			var valid = await this.$refs.dialogForm.validate().catch(() => {});
			if (!valid) {
				return false;
			}
			this.isSaveing = true;
			var res = {};
			if (this.mode == "add") {
				delete this.form.id;
				res =
					await this.$API.codeGenerator.databaseEntityFieldType.createDatabaseEntityFieldType.post(
						this.form
					);
			} else {
				res =
					await this.$API.codeGenerator.databaseEntityFieldType.updateDatabaseEntityFieldType.post(
						this.form
					);
			}

			console.log("res", res);
			this.isSaveing = false;
			if (res.code == 20000) {
				this.$emit("success", this.form, this.mode);
				this.visible = false;
				this.$message.success("操作成功");
			} else {
				this.$alert(res.msg, "提示", { type: "error" });
			}
		},
		//表单注入数据
		setData(data) {
			//可以和上面一样单个注入，也可以像下面一样直接合并进去
			Object.assign(this.form, data);
		},
	},
};
</script>

<style></style>
