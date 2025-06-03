import config from "@/config";
import http from "@/utils/request";

export default {
	menu: {
		myMenus: {
			url: `${config.MY_API_URL}/menu/GetMenus`,
			name: "获取我的菜单",
			get: async function () {
				return await http.get(this.url);
			},
		},
		getApiList: {
			url: `${config.MY_API_URL}/menu/GetApiList`,
			name: "获取我的菜单",
			get: async function () {
				return await http.get(this.url);
			},
		},
		list: {
			url: `${config.MY_API_URL}/menu/list`,
			name: "获取菜单",
			get: async function () {
				return await http.get(this.url);
			},
		},
		addMenu: {
			url: `${config.MY_API_URL}/menu/addMenu`,
			name: "添加菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		updateMenu: {
			url: `${config.MY_API_URL}/menu/updateMenu`,
			name: "添加菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		deleteMenus: {
			url: `${config.MY_API_URL}/menu/deleteMenus`,
			name: "删除菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
	},
	dic: {
		tree: {
			url: `${config.MY_API_URL}/Dictionary/List`,
			name: "获取字典树",
			get: async function () {
				return await http.get(this.url);
			},
		},
		listItem: {
			url: `${config.MY_API_URL}/Dictionary/listItem`,
			name: "字典明细",
			post: async function (params) {
				return await http.post(this.url, params);
			},
		},
		getDictionaryItems: {
			url: `${config.MY_API_URL}/Dictionary/GetDictionaryItems`,
			name: "根据字典编号获取字典数据项列表",
			get: async function (code) {
				var params = {};
				params.code = code;
				return await http.get(this.url, params);
			},
		},
		get: {
			url: `${config.API_URL}/system/dic/get`,
			name: "获取字典数据",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
		addDic: {
			url: `${config.MY_API_URL}/Dictionary/addDictionary`,
			name: "添加菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		updateDic: {
			url: `${config.MY_API_URL}/Dictionary/updateDictionary`,
			name: "添加菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		deleteDics: {
			url: `${config.MY_API_URL}/Dictionary/DeleteDictionarys`,
			name: "删除菜单",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		addDicItem: {
			url: `${config.MY_API_URL}/Dictionary/addDictionaryItem`,
			name: "添加字典项",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		updateDicItem: {
			url: `${config.MY_API_URL}/Dictionary/updateDictionaryItem`,
			name: "添加字典项",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
		deleteDicItems: {
			url: `${config.MY_API_URL}/Dictionary/DeleteDictionaryItems`,
			name: "删除字典项",
			post: async function (data) {
				return await http.post(this.url, data, {
					headers: {
						//'response-status': 401
					},
				});
			},
		},
	},
	role: {
		list: {
			url: `${config.API_URL}/system/role/list2`,
			name: "获取角色列表",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
	dept: {
		list: {
			url: `${config.API_URL}/system/dept/list`,
			name: "获取部门列表",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
	user: {
		list: {
			url: `${config.API_URL}/system/user/list`,
			name: "获取用户列表",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
	app: {
		list: {
			url: `${config.API_URL}/system/app/list`,
			name: "应用列表",
			get: async function () {
				return await http.get(this.url);
			},
		},
	},
	log: {
		list: {
			url: `${config.API_URL}/system/log/list`,
			name: "日志列表",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
	table: {
		list: {
			url: `${config.API_URL}/system/table/list`,
			name: "表格列管理列表",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
		info: {
			url: `${config.API_URL}/system/table/info`,
			name: "表格列管理详情",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
	tasks: {
		list: {
			url: `${config.API_URL}/system/tasks/list`,
			name: "系统任务管理",
			get: async function (params) {
				return await http.get(this.url, params);
			},
		},
	},
};
