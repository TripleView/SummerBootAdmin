<template>
	<el-container>
		<el-header height="auto">
			<div class="left-panel">
				<el-form :inline="true" :model="query" class="demo-form-inline">
					<el-form-item label="字段类型名称">
						<el-input
							v-model="query.name"
							placeholder="请输入字段类型名称"
							clearable
						/>
					</el-form-item>
					<el-form-item label="字段类型">
						<el-input
							v-model="query.value"
							placeholder="请输入字段类型"
							clearable
						/>
					</el-form-item>

					<el-form-item label="描述">
						<el-input
							v-model="query.description"
							placeholder="请输入描述"
							clearable
						/>
					</el-form-item>

					<el-form-item>
						<el-button
							type="primary"
							icon="el-icon-search"
							@click="getList"
							>查询</el-button
						>
						<el-button
							type="primary"
							icon="el-icon-plus"
							@click="create"
							>新增</el-button
						>

						<el-button
							type="danger"
							plain
							icon="el-icon-delete"
							:disabled="selection.length == 0"
							@click="batch_del"
							>批量删除</el-button
						>
					</el-form-item>
				</el-form>
			</div>
		</el-header>
		<el-main class="nopadding">
			<scTable
				ref="table"
				:apiObj="list.apiObj"
				:params="query"
				row-key="id"
				@selection-change="selectionChange"
				httpMethod="post"
				stripe
			>
				<el-table-column type="selection" width="50"></el-table-column>
				<el-table-column
					label="排序"
					prop="orderIndex"
					width="100"
				></el-table-column>
				<el-table-column
					label="字段类型名称"
					prop="name"
					width="180"
				></el-table-column>
				<el-table-column
					label="字段类型"
					prop="value"
					width="150"
				></el-table-column>
				<el-table-column
					label="描述"
					prop="description"
					width="250"
				></el-table-column>
				<el-table-column label="操作" fixed="right" width="300">
					<template #default="scope">
						<el-button
							type="primary"
							plain
							size="small"
							@click="table_edit(scope.row)"
							>编辑</el-button
						>

						<el-popconfirm
							title="确定删除吗？"
							@confirm="table_del(scope.row, scope.$index)"
						>
							<template #reference>
								<el-button plain type="danger" size="small"
									>删除</el-button
								>
							</template>
						</el-popconfirm>
					</template>
				</el-table-column>
			</scTable>
		</el-main>
	</el-container>

	<save-dialog
		v-if="dialog.save"
		ref="saveDialog"
		@success="getList"
		@closed="dialog.save = false"
	></save-dialog>
</template>

<script>
import saveDialog from "./save";

export default {
	name: "fieldTypeSetting",
	components: {
		saveDialog,
	},
	data() {
		return {
			query: {
				name: "",
				value: "",
				description: "",
			},
			dialog: {
				save: false,
				info: false,
			},
			list: {
				apiObj: this.$API.codeGenerator.databaseEntityFieldType
					.getDatabaseEntityFieldTypesByPage,
			},
			selection: [],
		};
	},
	mounted() {},
	methods: {
		getList() {
			this.$refs["table"].refresh();
		},
		//窗口新增
		create() {
			this.dialog.save = true;
			this.$nextTick(() => {
				this.$refs.saveDialog.open();
			});
		},
		//窗口编辑
		table_edit(row) {
			this.dialog.save = true;
			this.$nextTick(() => {
				this.$refs.saveDialog.open("edit").setData(row);
			});
		},
		//查看
		table_show(row) {
			this.dialog.info = true;
			this.$nextTick(() => {
				this.$refs.infoDialog.setData(row);
			});
		},
		//删除明细
		async table_del(row, index) {
			var ids = [row.id];
			await this.internalBatchDelete(ids);
		},
		//批量删除
		async batch_del() {
			var confirmRes = await this.$confirm(
				`确定删除选中的 ${this.selection.length} 项吗？`,
				"提示",
				{
					type: "warning",
					confirmButtonText: "删除",
					confirmButtonClass: "el-button--danger",
				}
			).catch(() => {});

			if (!confirmRes) {
				return false;
			}

			var ids = this.selection.map((v) => v.id);
			await this.internalBatchDelete(ids);
		},
		async internalBatchDelete(ids) {
			var reqData = { ids: ids };
			var res =
				await this.$API.codeGenerator.databaseEntityFieldType.deleteDatabaseEntityFieldTypes.post(
					reqData
				);
			if (res.code == 20000) {
				this.$message.success("删除成功");
				this.getList();
			} else {
				this.$alert(res.message, "提示", { type: "error" });
			}
		},
		//表格选择后回调事件
		selectionChange(selection) {
			this.selection = selection;
		},
	},
};
</script>

<style></style>
