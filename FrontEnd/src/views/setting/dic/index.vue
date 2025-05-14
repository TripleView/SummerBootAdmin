<template>
	<el-container>
		<el-aside width="300px" v-loading="showDicloading">
			<el-container>
				<el-header>
					<el-input placeholder="输入关键字进行过滤" v-model="dicFilterText" clearable></el-input>
				</el-header>
				<el-main class="nopadding">
					<el-tree ref="dic" class="menu" node-key="id" :data="dicList" :props="dicProps" :highlight-current="true"
						:expand-on-click-node="false" :filter-node-method="dicFilterNode" @node-click="resetItems">
						<template #default="{ node, data }">
							<span class="custom-tree-node">
								<span class="label">{{ node.label }}</span>
								<span class="code">{{ data.code }}</span>
								<span class="do">
									<el-button-group>
										<el-button icon="el-icon-plus" size="small" @click.stop="addSubDic(data)"></el-button>
										<el-button icon="el-icon-edit" size="small" @click.stop="editDic(data)"></el-button>
										<el-button icon="el-icon-delete" size="small" @click.stop="deleteDic(node, data)"></el-button>
									</el-button-group>
								</span>
							</span>
						</template>
					</el-tree>
				</el-main>
				<el-footer style="height:51px;">
					<el-button type="primary" size="small" icon="el-icon-plus" style="width: 100%;" @click="addDic">字典分类</el-button>
				</el-footer>
			</el-container>
		</el-aside>
		<el-container class="is-vertical">
			<el-header>
				<div class="left-panel">
					<el-button type="primary" icon="el-icon-plus" @click="addInfo"></el-button>
					<el-button type="danger" plain icon="el-icon-delete" :disabled="selection.length == 0"
						@click="batch_del"></el-button>
				</div>
			</el-header>
			<el-main class="nopadding">

				<sbTable ref="table" :apiObj="listApi" row-key="id" :params="listApiParams" @selection-change="selectionChange"
					stripe :paginationLayout="'prev, pager, next'">
					<el-table-column type="selection" width="50"></el-table-column>
					<el-table-column label="" width="60">
						<template #default>
							<el-tag class="move" style="cursor: move;"><el-icon-d-caret style="width: 1em; height: 1em;" /></el-tag>
						</template>
					</el-table-column>
					<el-table-column label="名称" prop="name" width="150"></el-table-column>
					<el-table-column label="键值" prop="value" width="150"></el-table-column>
					<el-table-column label="排序" prop="index" width="150"></el-table-column>
					<!-- <el-table-column label="是否有效" prop="yx" width="100">
						<template #default="scope">
							<el-switch v-model="scope.row.yx" @change="changeSwitch($event, scope.row)" :loading="scope.row.$switch_yx" active-value="1" inactive-value="0"></el-switch>
						</template>
					</el-table-column> -->
					<el-table-column label="操作" fixed="right" align="right" width="120">
						<template #default="scope">
							<el-button-group>
								<el-button text type="primary" size="small" @click="table_edit(scope.row, scope.$index)">编辑</el-button>
								<el-popconfirm title="确定删除吗？" @confirm="deleteDicItem(scope.row, scope.$index)">
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

	<dic-dialog v-if="dialog.dic" ref="dicDialog" @success="handleDicSuccess" @closed="dialog.dic = false"></dic-dialog>

	<list-dialog v-if="dialog.list" ref="listDialog" @success="handleListSuccess"
		@closed="dialog.list = false"></list-dialog>
</template>

<script>
import dicDialog from './dic'
import listDialog from './list'
import Sortable from 'sortablejs'

export default {
	name: 'dic',
	components: {
		dicDialog,
		listDialog
	},
	data() {
		return {

			dialog: {
				dic: false,
				info: false,
				list: false
			},
			showDicloading: true,
			dicList: [],
			dicFilterText: '',
			dicProps: {
				label: 'name'
			},
			listApi: null,
			listApiParams: {},
			selection: []
		}
	},
	watch: {
		dicFilterText(val) {
			this.$refs.dic.filter(val);
		}
	},
	mounted() {
		this.getDic()
		this.rowDrop()
	},
	methods: {
		//加载树数据
		async getDic() {
			var res = await this.$API.system.dic.tree.get();
			this.showDicloading = false;
			this.dicList = res.data;
			this.listApiParams = {
				dictionaryId: 0
			}

			this.listApi = this.$API.system.dic.listItem;
		},
		setDefaultNode() {
			//获取第一个节点,设置选中 & 加载明细列表
			var firstNode = this.dicList[0];
			if (firstNode) {
				// alert(789)
				this.$nextTick(() => {
					this.$refs.dic.setCurrentKey(firstNode.id)
				})

			}
		},
		//树过滤
		dicFilterNode(value, data) {
			if (!value) return true;
			var targetText = data.name + data.code;
			return targetText.indexOf(value) !== -1;
		},
		//树增加
		addDic() {
			this.dialog.dic = true
			this.$nextTick(() => {
				this.$refs.dicDialog.open()
			})
		},
		//编辑树
		editDic(data) {
			this.dialog.dic = true
			this.$nextTick(() => {
				this.$refs.dicDialog.open('edit').setData(data)
			})
		},
		//添加子字典
		addSubDic(data) {
			this.dialog.dic = true
			this.$nextTick(() => {
				var subDic = {}
				subDic.parentId = data.id
				this.$refs.dicDialog.open().setData(subDic)
			})
		},
		resetItems() {
			var dicCurrentKey = this.$refs.dic.getCurrentKey()
			if (dicCurrentKey == null) {
				dicCurrentKey = 0
			}
			this.$refs.table.reload({
				dictionaryId: dicCurrentKey
			})
		},
		//删除树
		deleteDic(node, data) {
			console.log("node", this.node, data)

			this.$confirm(`确定删除 ${data.name} 项吗？`, '提示', {
				type: 'warning'
			}).then(async () => {
				this.showDicloading = true;
				var reqData = {}
				reqData.Ids = [];
				reqData.Ids.push(data.id);
				var res = await this.$API.system.dic.deleteDics.post(reqData);
				if (res.code == 20000) {
					this.$message.success("删除成功")
					await this.getDic()
					this.$refs.dic.setCurrentKey(data.parentId)
					this.resetItems()
				} else {
					this.$alert(res.msg, "提示", { type: 'error' })
				}

				this.showDicloading = false;
				// this.$message.success("操作成功")
			}).catch(() => {

			})
		},
		//行拖拽
		rowDrop() {
			const _this = this
			const tbody = this.$refs.table.$el.querySelector('.el-table__body-wrapper tbody')
			Sortable.create(tbody, {
				handle: ".move",
				animation: 300,
				ghostClass: "ghost",
				onEnd({ newIndex, oldIndex }) {
					const tableData = _this.$refs.table.tableData
					const currRow = tableData.splice(oldIndex, 1)[0]
					tableData.splice(newIndex, 0, currRow)
					_this.$message.success("排序成功")
				}
			})
		},
		//添加明细
		addInfo() {
			this.dialog.list = true
			this.$nextTick(() => {
				var dicCurrentKey = this.$refs.dic.getCurrentKey();

				const data = {
					dictionaryId: dicCurrentKey
				}
				console.log("字典值为", data)
				this.$refs.listDialog.open().setData(data)
			})
		},
		//编辑明细
		table_edit(row) {
			this.dialog.list = true
			this.$nextTick(() => {
				this.$refs.listDialog.open('edit').setData(row)
			})
		},
		//删除明细
		async deleteDicItem(row, index) {
			var reqData = {}
			reqData.Ids = [];
			reqData.Ids.push(row.id);
			console.log("this.$API.system.dic", this.$API.system.dic)
			//  {id: row.id}
			var res = await this.$API.system.dic.deleteDicItems.post(reqData);
			if (res.code == 20000) {
				this.$refs.table.reload({
					dictionaryId: row.dictionaryId
				})
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
				const loading = this.$loading();
				var dictionaryId = 0;
				this.selection.forEach(item => {
					reqData.Ids.push(item.id);
					dictionaryId = item.dictionaryId
				})

				var res = await this.$API.system.dic.deleteDicItems.post(reqData);

				if (res.code == 20000) {

					that.$refs.table.reload({
						dictionaryId: dictionaryId
					})
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
		//提交明细
		saveList() {
			this.$refs.listDialog.submit(async (formData) => {
				this.isListSaveing = true;
				var res = await this.$API.demo.post.post(formData);
				this.isListSaveing = false;
				if (res.code == 200) {
					//这里选择刷新整个表格 OR 插入/编辑现有表格数据
					this.listDialogVisible = false;
					this.$message.success("操作成功")
				} else {
					this.$alert(res.msg, "提示", { type: 'error' })
				}
			})
		},
		//表格选择后回调事件
		selectionChange(selection) {
			this.selection = selection;
		},
		//表格内开关事件
		changeSwitch(val, row) {
			//1.还原数据
			row.yx = row.yx == '1' ? '0' : '1'
			//2.执行加载
			row.$switch_yx = true;
			//3.等待接口返回后改变值
			setTimeout(() => {
				delete row.$switch_yx;
				row.yx = val;
				this.$message.success(`操作成功id:${row.id} val:${val}`)
			}, 500)
		},
		//本地更新数据
		async handleDicSuccess(data, mode) {
			console.log("handleDicSuccess", data, mode)
			await this.getDic();
			this.$refs.dic.setCurrentKey(data.id)
			this.resetItems()
			return;
			// if(mode=='add'){
			// 	data.id = new Date().getTime()
			// 	if(this.dicList.length > 0){
			// 		this.$refs.table.upData({
			// 			code: data.code
			// 		})
			// 	}else{
			// 		this.listApiParams = {
			// 			code: data.code
			// 		}
			// 		this.listApi = this.$API.dic.info;
			// 	}
			// 	this.$refs.dic.append(data, data.parentId[0])
			// 	this.$refs.dic.setCurrentKey(data.id)
			// }else if(mode=='edit'){
			// 	var editNode = this.$refs.dic.getNode(data.id);
			// 	//判断是否移动？
			// 	var editNodeParentId =  editNode.level==1?undefined:editNode.parent.data.id
			// 	if(editNodeParentId != data.parentId){
			// 		var obj = editNode.data;
			// 		this.$refs.dic.remove(data.id)
			// 		this.$refs.dic.append(obj, data.parentId[0])
			// 	}
			// 	Object.assign(editNode.data, data)
			// }
		},
		//本地更新数据
		handleListSuccess(data, mode) {
			console.log(data, mode)
			this.$refs.table.reload({
				dictionaryId: data.dictionaryId
			})
			return;
			if (mode == 'add') {
				data.id = new Date().getTime()
				this.$refs.table.tableData.push(data)
			} else if (mode == 'edit') {
				this.$refs.table.tableData.filter(item => item.id === data.id).forEach(item => {
					Object.assign(item, data)
				})
			}
		}
	}
}
</script>

<style scoped>
.menu:deep(.el-tree-node__label) {
	display: flex;
	flex: 1;
	height: 100%;
}

.custom-tree-node {
	display: flex;
	flex: 1;
	align-items: center;
	justify-content: space-between;
	font-size: 14px;
	padding-right: 24px;
	height: 100%;
}

.custom-tree-node .code {
	font-size: 12px;
	color: #999;
}

.custom-tree-node .do {
	display: none;
}

.custom-tree-node:hover .code {
	display: none;
}

.custom-tree-node:hover .do {
	display: inline-block;
}
</style>
