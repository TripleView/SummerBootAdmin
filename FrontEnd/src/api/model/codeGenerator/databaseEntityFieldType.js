import config from "@/config";
import http from "@/utils/request";

export default {
	list: {
		url: `${config.MY_API_URL}/DatabaseEntityFieldType/List`,
		name: "获取用户",
		get: async function () {
			return await http.get(this.url);
		},
	},
	getDatabaseEntityFieldTypesByPage: {
		url: `${config.MY_API_URL}/DatabaseEntityFieldType/GetDatabaseEntityFieldTypesByPage`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	createDatabaseEntityFieldType: {
		url: `${config.MY_API_URL}/DatabaseEntityFieldType/CreateDatabaseEntityFieldType`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	updateDatabaseEntityFieldType: {
		url: `${config.MY_API_URL}/DatabaseEntityFieldType/UpdateDatabaseEntityFieldType`,
		name: "添加用户",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	deleteDatabaseEntityFieldTypes: {
		url: `${config.MY_API_URL}/DatabaseEntityFieldType/DeleteDatabaseEntityFieldTypes`,
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
