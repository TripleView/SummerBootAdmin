<!--
 * @Descripttion: 代码编辑器
 * @version: 1.0
 * @Author: sakuya
 * @Date: 2022年5月20日21:46:29
 * @LastEditors: 
 * @LastEditTime: 
-->

<template>
	<div class="sc-code-editor" :style="mystyle">
		<textarea ref="textarea" v-model="contentValue"></textarea>
	</div>
</template>

<script>
import { markRaw } from "vue"

//框架
import CodeMirror from 'codemirror'
import 'codemirror/lib/codemirror.css'

//主题
import 'codemirror/theme/idea.css'
import 'codemirror/theme/darcula.css'

//功能
import 'codemirror/addon/selection/active-line'

//语言
import 'codemirror/mode/javascript/javascript'
import 'codemirror/mode/sql/sql'
import 'codemirror/mode/shell/shell'

export default {
	props: {
		modelValue: {
			type: String,
			default: ""
		},
		mode: {
			type: String,
			default: "javascript"
		},
		height: {
			type: [String, Number],
			default: 300,
		},
		width: {
			type: [String, Number],
			default: "100%",
		},
		options: {
			type: Object,
			default: () => { }
		},
		theme: {
			type: String,
			default: "idea"
		},
		readOnly: {
			type: Boolean,
			default: false
		},
	},
	data() {
		return {
			contentValue: this.modelValue,
			coder: null,
			opt: {
				// 显示函数
				line: true,
				autoRefresh: true,
				theme: this.theme,	//主题
				styleActiveLine: true,	//高亮当前行
				lineNumbers: true,	//行号
				lineWrapping: false,	//自动换行
				tabSize: 4,	//Tab缩进
				indentUnit: 4,	//缩进单位
				indentWithTabs: true,	//自动缩进
				mode: this.mode,	//语言
				readOnly: this.readOnly,	//只读
				placeholder: "请输入脚本",
				defaultTextHeight: '21px',
				singleCursorHeightPerLine: false,
				cursorHeight: 1,
				// 软换行
				// lineWrapping: true,
				...this.options
			}
		}
	},
	computed: {
		mystyle() {
			return { 'height': this._height, "width": this._width }
		},
		_height() {
			return Number(this.height) ? Number(this.height) + 'px' : this.height
		},
		_width() {

			return Number(this.width) ? Number(this.width) + 'px' : this.width
		},
	},
	watch: {
		modelValue(val) {
			var that = this;
			this.contentValue = val
			if (val !== this.coder.getValue()) {
				this.coder.setValue(val)
				this.$nextTick(() => {
					that.coder.refresh();
				})

			}
		}
	},
	mounted() {

		this.init()
		//获取挂载的所有modes
		//console.log(CodeMirror.modes)
	},
	methods: {
		init() {
			var that = this;
			this.coder = markRaw(CodeMirror.fromTextArea(this.$refs.textarea, this.opt))
			this.coder.on('change', (coder) => {
				this.contentValue = coder.getValue()
				this.$emit('update:modelValue', this.contentValue)
			})
		},
		formatStrInJson(strValue) {
			return JSON.stringify(JSON.parse(strValue), null, 4)
		},
		refresh() {
			var that = this;
			this.$nextTick(() => {
				that.coder && that.coder.refresh();
			})
		}
	}
}
</script>

<style scoped>
.sc-code-editor {
	font-size: 14px;
	border: 1px solid #ddd;
	line-height: 150%;
	width: 100%;
}

.sc-code-editor:deep(.CodeMirror) {
	height: 100%;
}
</style>
