<!--
 * @Descripttion: 目录选择器组件
 * @version: 1.0
 * @Author: opencode
 * @Date: 2026年8月27日
 * @LastEditors: opencode
 * @LastEditTime: 2026年8月27日
-->

<template>
	<div class="sc-directory-select">
		<el-select
			ref="select"
			v-model="selectedValue"
			:size="size"
			:clearable="clearable"
			:placeholder="placeholder"
			:disabled="disabled"
			:filterable="filterable"
			:filter-method="filterMethod"
			@visible-change="visibleChange"
			@clear="clear"
		>
			<template #empty>
				<div class="sc-directory-select__tree" v-loading="loading">
					<el-tree
						ref="tree"
						:data="treeData"
						:props="treeProps"
						:node-key="nodeKey"
						:highlight-current="true"
						:expand-on-click-node="false"
						:default-expanded-keys="defaultExpandedKeys"
						:filter-node-method="filterNode"
						@node-click="handleNodeClick"
					>
						<template #default="{ node, data }">
							<span class="sc-directory-select__tree-node">
								<el-icon class="sc-directory-select__tree-icon">
									<el-icon-folder />
								</el-icon>
								<span>{{ node.label }}</span>
							</span>
						</template>
					</el-tree>
				</div>
			</template>
		</el-select>
	</div>
</template>

<script>
export default {
	name: "scDirectorySelect",
	props: {
		modelValue: {
			type: [String, Number, Object],
			default: "",
		},
		apiObj: {
			type: Object,
			default: () => ({}),
		},
		params: {
			type: Object,
			default: () => ({}),
		},
		placeholder: {
			type: String,
			default: "请选择目录",
		},
		size: {
			type: String,
			default: "default",
		},
		clearable: {
			type: Boolean,
			default: true,
		},
		disabled: {
			type: Boolean,
			default: false,
		},
		filterable: {
			type: Boolean,
			default: true,
		},
		nodeKey: {
			type: String,
			default: "fullName",
		},
		props: {
			type: Object,
			default: () => ({
				label: "name",
				children: "children",
			}),
		},
		valueType: {
			type: String,
			default: "key",
			validator: (value) => ["key", "object", "path"].includes(value),
		},
	},
	emits: ["update:modelValue", "change"],
	data() {
		return {
			loading: false,
			treeData: [],
			selectedValue: "",
			selectedLabel: "",
			selectedNode: null,
			defaultExpandedKeys: [],
			treeProps: {
				label: this.props.label || "name",
				children: this.props.children || "children",
			},
		};
	},
	watch: {
		modelValue: {
			handler(val) {
				this.setSelectedValue(val);
			},
			immediate: true,
		},
	},
	mounted() {
		this.getTreeData();
	},
	methods: {
		async getTreeData() {
			if (!this.apiObj || !this.apiObj.get) {
				return;
			}
			this.loading = true;
			try {
				const res = await this.apiObj.get(this.params);

				if (res.code == 20000) {
					const data = res.data;
					// 兼容 data 为数组或单个对象的情况
					if (Array.isArray(data)) {
						this.treeData = data;
					} else if (data && typeof data === "object") {
						this.treeData = [data];
					} else {
						this.treeData = [];
					}
					this.setDefaultExpandedKeys();
					this.setSelectedValue(this.modelValue);
				}
			} catch (error) {
				console.error("获取目录数据失败:", error);
			} finally {
				this.loading = false;
			}
		},
		setDefaultExpandedKeys() {
			if (this.treeData.length > 0) {
				this.defaultExpandedKeys = [this.treeData[0][this.nodeKey]];
			}
		},
		setSelectedValue(val) {
			if (!val) {
				this.selectedValue = "";
				this.selectedLabel = "";
				this.selectedNode = null;
				return;
			}

			if (this.valueType === "object" && typeof val === "object") {
				this.selectedValue = val[this.nodeKey];
				this.selectedLabel = val[this.treeProps.label];
				this.selectedNode = val;
			} else if (this.valueType === "path" && typeof val === "string") {
				this.selectedValue = val;
				this.selectedLabel = this.findLabelByPath(val);
			} else {
				this.selectedValue = val;
				this.selectedLabel = this.findLabelByKey(val);
			}
		},
		findLabelByKey(key) {
			const node = this.findNodeByKey(this.treeData, key);
			return node ? node[this.treeProps.label] : "";
		},
		findLabelByPath(path) {
			const node = this.findNodeByPath(this.treeData, path);
			return node ? node[this.treeProps.label] : "";
		},
		findNodeByKey(data, key) {
			for (let i = 0; i < data.length; i++) {
				const item = data[i];
				if (item[this.nodeKey] === key) {
					return item;
				}
				if (
					item[this.treeProps.children] &&
					item[this.treeProps.children].length > 0
				) {
					const found = this.findNodeByKey(
						item[this.treeProps.children],
						key
					);
					if (found) {
						return found;
					}
				}
			}
			return null;
		},
		findNodeByPath(data, path, parentPath = "") {
			for (let i = 0; i < data.length; i++) {
				const item = data[i];
				const currentPath = parentPath
					? `${parentPath}/${item[this.treeProps.label]}`
					: item[this.treeProps.label];
				if (currentPath === path) {
					return item;
				}
				if (
					item[this.treeProps.children] &&
					item[this.treeProps.children].length > 0
				) {
					const found = this.findNodeByPath(
						item[this.treeProps.children],
						path,
						currentPath
					);
					if (found) {
						return found;
					}
				}
			}
			return null;
		},
		handleNodeClick(data) {
			this.selectedNode = data;
			this.selectedLabel = data[this.treeProps.label];

			let emitValue;
			if (this.valueType === "object") {
				emitValue = data;
			} else if (this.valueType === "path") {
				emitValue = this.getNodePath(data);
			} else {
				emitValue = data[this.nodeKey];
			}

			this.selectedValue =
				this.valueType === "object" ? data[this.nodeKey] : emitValue;
			this.$refs.select.blur();
			this.$emit("update:modelValue", emitValue);
			this.$emit("change", emitValue, data);
		},
		getNodePath(node) {
			const pathParts = [];
			const findPath = (data, target, parentPath = "") => {
				for (let i = 0; i < data.length; i++) {
					const item = data[i];
					const currentPath = parentPath
						? `${parentPath}/${item[this.treeProps.label]}`
						: item[this.treeProps.label];
					if (item[this.nodeKey] === target[this.nodeKey]) {
						pathParts.push(currentPath);
						return true;
					}
					if (
						item[this.treeProps.children] &&
						item[this.treeProps.children].length > 0
					) {
						if (
							findPath(
								item[this.treeProps.children],
								target,
								currentPath
							)
						) {
							return true;
						}
					}
				}
				return false;
			};
			findPath(this.treeData, node);
			return pathParts[0] || "";
		},
		filterNode(value, data) {
			if (!value) return true;
			return data[this.treeProps.label].includes(value);
		},
		filterMethod(keyword) {
			this.$refs.tree.filter(keyword);
		},
		visibleChange(visible) {
			if (visible && this.treeData.length === 0) {
				this.getTreeData();
			}
		},
		clear() {
			this.selectedNode = null;
			this.$emit("update:modelValue", "");
			this.$emit("change", "", null);
		},
		refresh() {
			this.getTreeData();
		},
	},
};
</script>

<style scoped>
.sc-directory-select {
	display: inline-block;
	width: 100%;
}

.sc-directory-select__tree {
	padding: 10px;
	min-height: 200px;
	max-height: 400px;
	overflow-y: auto;
}

.sc-directory-select__tree-node {
	display: flex;
	align-items: center;
	font-size: 14px;
}

.sc-directory-select__tree-icon {
	margin-right: 8px;
	color: var(--el-color-primary);
}

:deep(.el-tree-node__content) {
	height: 36px;
}

:deep(
		.el-tree--highlight-current
			.el-tree-node.is-current
			> .el-tree-node__content
	) {
	background-color: var(--el-color-primary-light-9);
}
</style>
