<template>
	<el-container>
		<el-aside width="200px" v-loading="showGrouploading">
			<el-container>
				<el-header>
					<el-input placeholder="输入关键字进行过滤" v-model="groupFilterText" clearable></el-input>
				</el-header>
				<el-main class="nopadding">

					<el-tree ref="group" class="menu" node-key="id" :data="group" :current-node-key="''" :props="dicProps"
						:highlight-current="true" :expand-on-click-node="false" :filter-node-method="groupFilterNode"
						@node-click="departmentClick"></el-tree>
				</el-main>
			</el-container>
		</el-aside>
		<el-container>
			<el-header>
				<div class="left-panel">
					<el-button type="primary" icon="el-icon-plus" @click="add"></el-button>
					<el-button type="danger" plain icon="el-icon-delete" :disabled="selection.length == 0"
						@click="batch_del"></el-button>
					<el-button type="primary" plain :disabled="selection.length == 0" @click="assignRolesHandler">分配角色</el-button>
					<el-button type="primary" plain :disabled="selection.length == 0" @click="resetPasswordHandler">重置密码</el-button>
				</div>
				<div class="right-panel">
					<div class="right-panel-search">
						<el-input v-model="search.name" placeholder="登录账号 / 姓名" clearable></el-input>
						<el-button type="primary" icon="el-icon-search" @click="upsearch"></el-button>
					</div>
				</div>
			</el-header>
			<el-main class="nopadding">
				<sbTable ref="table" :apiObj="apiObj" @selection-change="selectionChange" stripe remoteSort remoteFilter>
					<el-table-column type="selection" width="50"></el-table-column>
					<el-table-column label="ID" prop="id" width="80" sortable='custom'></el-table-column>
					<!-- <el-table-column label="头像" width="80" column-key="filterAvatar"
						:filters="[{ text: '已上传', value: '1' }, { text: '未上传', value: '0' }]">
						<template #default="scope">
							<el-avatar :src="scope.row.avatar" size="small"></el-avatar>
						</template>
					</el-table-column> -->
					<el-table-column label="登录账号" prop="account" width="150" sortable='custom' column-key="filterUserName"
						:filters="[{ text: '系统账号', value: '1' }, { text: '普通账号', value: '0' }]"></el-table-column>
					<el-table-column label="姓名" prop="name" width="150" sortable='custom'></el-table-column>
					<el-table-column label="所属角色" prop="roleName" width="200" sortable='custom'></el-table-column>
					<el-table-column label="加入时间" prop="createOn" width="170" sortable='custom'>
						<template #default="scope">
							{{ formatLocalTime(scope.row.createOn) }}
						</template>
					</el-table-column>
					<el-table-column label="操作" fixed="right" align="right" width="160">
						<template #default="scope">
							<el-button-group>
								<el-button text type="primary" size="small" @click="table_show(scope.row, scope.$index)">查看</el-button>
								<el-button text type="primary" size="small" @click="table_edit(scope.row, scope.$index)">编辑</el-button>
								<el-popconfirm title="确定删除吗？" @confirm="table_del(scope.row, scope.$index)">
									<template #reference>
										<el-button text type="primary" size="small">删除</el-button>
									</template>
								</el-popconfirm>
							</el-button-group>
						</template>
					</el-table-column>

				</sbTable>
			</el-main>
		</el-container>
	</el-container>

	<save-dialog v-if="dialog.save" ref="saveDialog" @success="handleSuccess" @closed="dialog.save = false"></save-dialog>
	<assign-roles v-if="dialog.assignRoles" ref="assignRoles" @success="handleSuccess"
		@closed="dialog.assignRoles = false"></assign-roles>
	<resetPassword v-if="dialog.resetPassword" ref="resetPassword" @success="handleSuccess"
		@closed="dialog.resetPassword = false"></resetPassword>
</template>

<script>
import saveDialog from './save'
import assignRoles from './assignRoles'
import resetPassword from "./resetPassword"

export default {
	name: 'user',
	components: {
		saveDialog,
		assignRoles,
		resetPassword
	},
	data() {
		return {
			dialog: {
				save: false,
				assignRoles: false,
				resetPassword: false,
			},
			showGrouploading: false,
			groupFilterText: '',
			group: [],
			apiObj: this.$API.user.getUsersByPage,
			selection: [],
			currentDepartmentId: '',//当前部门id
			search: {
				name: null,
				departmentId: "",
			},
			dicProps: {
				value: "id",
				label: "name",
				emitPath: false,
				checkStrictly: true
			}
		}
	},
	watch: {
		groupFilterText(val) {
			this.$refs.group.filter(val);
		}
	},
	mounted() {
		this.getDepartment()
	},
	methods: {
		//重置密码被点击时
		resetPasswordHandler() {
			this.dialog.resetPassword = true;
			this.$nextTick(() => {
				this.$refs.resetPassword.open().setData(this.selection)
			})
		},
		//设置角色被点击时
		assignRolesHandler() {
			this.dialog.assignRoles = true;
			this.$nextTick(() => {
				this.$refs.assignRoles.open().setData(this.selection)
			})
		},
		formatLocalTime(utcString) {
			return this.$localDateFormat(utcString)
		},
		//添加
		add() {
			this.dialog.save = true
			this.$nextTick(() => {
				this.$refs.saveDialog.open().setDepartmentId(this.currentDepartmentId)
			})
		},
		//编辑
		table_edit(row) {
			this.dialog.save = true
			this.$nextTick(() => {
				this.$refs.saveDialog.open('edit').setData(row)
			})
		},
		//查看
		table_show(row) {
			this.dialog.save = true
			this.$nextTick(() => {
				this.$refs.saveDialog.open('show').setData(row)
			})
		},
		//删除
		async table_del(row) {

			var reqData = {}
			reqData.Ids = [];
			reqData.Ids.push(row.id);
			//  {id: row.id}
			var res = await this.$API.user.deleteUsers.post(reqData);
			if (res.code == 20000) {
				this.$refs.table.refresh()
				// this.$refs.table.tableData.splice(index, 1);
				this.$message.success("删除成功")
			} else {
				this.$alert(res.msg, "提示", { type: 'error' })
			}
		},
		//批量删除
		async batch_del() {
			var that = this;
			this.$confirm(`确定删除选中的 ${this.selection.length} 项吗？`, '提示', {
				type: 'warning'
			}).then(async () => {


				var reqData = {}
				reqData.Ids = [];
				const loading = that.$loading();

				that.selection.forEach(item => {
					reqData.Ids.push(item.id);

				})

				var res = await that.$API.user.deleteUsers.post(reqData);

				if (res.code == 20000) {
					that.$refs.table.refresh()
					// this.$refs.table.tableData.splice(index, 1);
					that.$message.success("删除成功")
					console.log("返回值123", res)
					loading.close();
				} else {
					this.$alert(res.msg, "提示", { type: 'error' })
					loading.close();
				}
			}).catch(() => {

			})
		},
		//表格选择后回调事件
		selectionChange(selection) {
			this.selection = selection;
		},
		//加载部门数据
		async getDepartment() {
			this.showGrouploading = true;
			var res = await this.$API.department.tree.get();
			this.showGrouploading = false;
			var allNode = { id: '', name: '所有' }
			res.data.unshift(allNode);
			this.group = res.data;
		},
		//树过滤
		groupFilterNode(value, data) {
			if (!value) return true;
			return data.label.indexOf(value) !== -1;
		},
		//部门点击事件
		departmentClick(data) {
			this.currentDepartmentId = data.id;
			this.search.departmentId = data.id
			this.$refs.table.reload({
				departmentId: data.id
			})
			// this.$refs.table.reload(params)
		},
		//搜索
		upsearch() {
			this.$refs.table.upData(this.search)
		},
		//本地更新数据
		handleSuccess(data, mode) {
			this.$refs.table.refresh();
			// if (mode == 'add') {
			// 	data.id = new Date().getTime()
			// 	this.$refs.table.tableData.unshift(data)
			// } else if (mode == 'edit') {
			// 	this.$refs.table.tableData.filter(item => item.id === data.id).forEach(item => {
			// 		Object.assign(item, data)
			// 	})
			// }
		}
	}
}
</script>

<style></style>
