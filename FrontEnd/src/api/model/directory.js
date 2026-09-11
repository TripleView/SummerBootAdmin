import config from "@/config";
import http from "@/utils/request";

export default {
	/**
	 * 获取目录树结构
	 * @param {Object} params - 查询参数
	 * @returns {Promise}
	 */
	getTree: {
		url: `${config.MY_API_URL}/directory/tree`,
		name: "获取目录树",
		get: async function(params) {
			return await http.get(this.url, params);
		}
	},

	/**
	 * 获取目录列表
	 * @param {Object} params - 查询参数
	 * @returns {Promise}
	 */
	list: {
		url: `${config.MY_API_URL}/directory/list`,
		name: "获取目录列表",
		get: async function(params) {
			return await http.get(this.url, params);
		}
	},

	/**
	 * 获取目录详情
	 * @param {String|Number} id - 目录ID
	 * @returns {Promise}
	 */
	get: {
		url: `${config.MY_API_URL}/directory/get`,
		name: "获取目录详情",
		get: async function(params) {
			return await http.get(this.url, params);
		}
	},

	/**
	 * 添加目录
	 * @param {Object} data - 目录数据
	 * @returns {Promise}
	 */
	add: {
		url: `${config.MY_API_URL}/directory/add`,
		name: "添加目录",
		post: async function(data) {
			return await http.post(this.url, data);
		}
	},

	/**
	 * 更新目录
	 * @param {Object} data - 目录数据
	 * @returns {Promise}
	 */
	update: {
		url: `${config.MY_API_URL}/directory/update`,
		name: "更新目录",
		post: async function(data) {
			return await http.post(this.url, data);
		}
	},

	/**
	 * 删除目录
	 * @param {Array|Object} data - 目录ID或ID数组
	 * @returns {Promise}
	 */
	delete: {
		url: `${config.MY_API_URL}/directory/delete`,
		name: "删除目录",
		post: async function(data) {
			return await http.post(this.url, data);
		}
	},

	/**
	 * 获取子目录列表
	 * @param {String|Number} parentId - 父目录ID
	 * @returns {Promise}
	 */
	getChildren: {
		url: `${config.MY_API_URL}/directory/children`,
		name: "获取子目录",
		get: async function(params) {
			return await http.get(this.url, params);
		}
	}
};
