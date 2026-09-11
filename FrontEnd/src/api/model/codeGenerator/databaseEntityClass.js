import config from "@/config";
import http from "@/utils/request";

export default {
	list: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/List`,
		name: "获取用户",
		get: async function () {
			return await http.get(this.url);
		},
	},
	getDatabaseEntityFields: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/GetDatabaseEntityFields`,
		name: "获取用户",
		get: async function (param) {
			return await http.get(this.url, param);
		},
	},
	getDatabaseEntityClasssByPage: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/GetDatabaseEntityClasssByPage`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	createDatabaseEntityClass: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/CreateDatabaseEntityClass`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	updateDatabaseEntityClass: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/UpdateDatabaseEntityClass`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	deleteDatabaseEntityClasss: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/DeleteDatabaseEntityClasss`,
		name: "删除用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	generateModel: {
		url: `${config.MY_API_URL}/DatabaseEntityClass/generateModel`,
		name: "删除用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
};
