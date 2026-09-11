<template>
	<el-dialog
		:title="titleMap[mode]"
		v-model="visible"
		width="100%"
		destroy-on-close
		@closed="$emit('closed')"
	>
		<el-form
			:model="form"
			:rules="rules"
			:disabled="mode == 'show'"
			ref="dialogForm"
			label-width="150px"
			label-position="left"
		>
			<el-form-item label="类名称" prop="name">
				<el-input
					v-model="form.name"
					placeholder="请输入类名称"
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
			<el-form-item label="字段列表" prop="description">
				<el-table
					:data="form.fields"
					style="width: 100%"
					ref="fieldsTable"
					row-key="id"
				>
					<el-table-column label="排序" width="80" align="center">
						<template #default>
							<span class="drag-handle">☷</span>
						</template>
					</el-table-column>
					<!-- 字段名：输入框 -->
					<el-table-column prop="name" label="字段名" width="250">
						<template #default="{ row }">
							<el-input
								v-model="row.name"
								placeholder="请输入字段名"
								clearable
							/>
						</template>
					</el-table-column>

					<!-- 字段类型：下拉框 -->
					<el-table-column
						prop="fieldTypeId"
						label="字段类型"
						width="180"
					>
						<template #default="{ row }">
							<el-select
								v-model="row.fieldTypeId"
								placeholder="请选择字段类型"
								clearable
								filterable
								style="width: 100%"
							>
								<el-option
									v-for="item in fieldTypeOptions"
									:key="item.id"
									:label="item.name"
									:value="item.id"
								>
									<span style="float: left">{{
										item.value
									}}</span>
									<span
										style="
											float: right;
											color: var(
												--el-text-color-secondary
											);
											font-size: 13px;
										"
									>
										{{ item.name }}
									</span>
								</el-option>
							</el-select>
						</template>
					</el-table-column>

					<!-- 是否可空：开关 -->
					<el-table-column
						prop="isEmpty"
						label="是否可空"
						width="180"
					>
						<template #default="{ row }">
							<el-switch
								v-model="row.isEmpty"
								inline-prompt
								active-text="是"
								inactive-text="否"
							/>
						</template>
					</el-table-column>

					<!-- 描述：输入框 -->
					<el-table-column
						prop="description"
						label="描述"
						min-width="220"
					>
						<template #default="{ row }">
							<el-input
								v-model="row.description"
								placeholder="请输入描述"
								clearable
							/>
						</template>
					</el-table-column>

					<!-- 删除按钮 -->
					<el-table-column prop="operator" label="操作">
						<template #default="{ row }">
							<el-icon
								size="20"
								@click="deleteField(row)"
								style="cursor: pointer"
								><Delete
							/></el-icon>
						</template>
					</el-table-column>
				</el-table>
				<el-row class="button-row" justify="center">
					<el-button
						@click="createField"
						style="margin-top: 10px; width: 200px"
						type="primary"
						>新增</el-button
					>
				</el-row>
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
import Sortable from "sortablejs";
import { Delete } from "@element-plus/icons-vue";
export default {
	emits: ["success", "closed"],
	components: {
		Delete,
	},
	data() {
		return {
			sortableInstance: null,
			fieldTypeOptions: [], //字段类型列表
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
				description: "",
				fields: [], //字段列表
			},
			//验证规则
			rules: {
				name: [{ required: true, message: "请输入字段类型名称" }],
				value: [{ required: true, message: "请输入字段类型" }],
			},
		};
	},
	mounted() {
		this.init();
	},
	beforeUnmount() {
		if (this.sortableInstance) {
			this.sortableInstance.destroy();
			this.sortableInstance = null;
		}
	},
	methods: {
		deleteField(row) {
			var index = this.form.fields.findIndex((x) => x.name == row.name);
			this.form.fields.splice(index, 1);
		},
		sortableFieldTable() {
			this.$nextTick(() => {
				console.log(" this.$refs.fieldsTable", this.$refs.fieldsTable);

				const tableRef = this.$refs.fieldsTable;

				// 防止 table 还没有渲染完成
				if (!tableRef) {
					console.warn("el-table 尚未渲染");
					return;
				}
				const tableElement = tableRef.$el;
				const tbody = tableElement.querySelector(
					".el-table__body-wrapper tbody"
				);

				if (!tbody) {
					console.warn("tbody尚未渲染");
					return;
				}

				this.sortableInstance = Sortable.create(tbody, {
					animation: 150,

					// 只有拖拽手柄可以拖动
					handle: ".drag-handle",

					// 拖拽过程中的样式
					ghostClass: "sortable-ghost",

					onEnd: ({ oldIndex, newIndex }) => {
						if (
							oldIndex === undefined ||
							newIndex === undefined ||
							oldIndex === newIndex
						) {
							return;
						}

						// 调整 tableData 中的数据顺序
						const movedRow = this.form.fields.splice(
							oldIndex,
							1
						)[0];

						this.form.fields.splice(newIndex, 0, movedRow);

						console.log("拖拽后的数据：", this.form.fields);
					},
				});
			});
		},
		async init() {
			var fieldTypeOptionsRes =
				await this.$API.codeGenerator.databaseEntityFieldType.list.get();
			if (fieldTypeOptionsRes.code == 20000) {
				this.fieldTypeOptions = fieldTypeOptionsRes.data;
			}
		},
		createField() {
			if (this.form.fields == null) {
				this.form.fields = [];
			}
			this.form.fields.push({
				name: "",
				fieldTypeId: "",
				isEmpty: null,
				description: "",
			});
		},
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
					await this.$API.codeGenerator.databaseEntityClass.createDatabaseEntityClass.post(
						this.form
					);
			} else {
				res =
					await this.$API.codeGenerator.databaseEntityClass.updateDatabaseEntityClass.post(
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
		async setData(data) {
			//可以和上面一样单个注入，也可以像下面一样直接合并进去
			Object.assign(this.form, data);
			var res =
				await this.$API.codeGenerator.databaseEntityClass.getDatabaseEntityFields.get(
					{ classId: this.form.id }
				);
			if (res.code == 20000) {
				this.form.fields = res.data;
			}
			console.log("res", res);
			this.sortableFieldTable();
		},
	},
};
</script>

<style scoped>
.button-row {
	width: 100%;
}

.drag-handle {
	cursor: move;
	user-select: none;
	font-size: 20px;
	color: #909399;
}

.drag-handle:hover {
	color: #409eff;
}

:deep(.sortable-ghost) {
	opacity: 0.5;
	background: #ecf5ff;
}
</style>
