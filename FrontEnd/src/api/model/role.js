import config from "@/config";
import http from "@/utils/request";

export default {
	list: {
		url: `${config.MY_API_URL}/Role/List`,
		name: "获取角色",
		get: async function () {
			return await http.get(this.url);
		},
	},
	getRolePermissions: {
		url: `${config.MY_API_URL}/Role/GetRolePermissions`,
		name: "获取角色",
		get: async function (roleId) {
			return await http.get(this.url, { roleId });
		},
	},
	getRolesByPage: {
		url: `${config.MY_API_URL}/Role/GetRolesByPage`,
		name: "添加角色",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	addRole: {
		url: `${config.MY_API_URL}/Role/AddRole`,
		name: "添加角色",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	updateRole: {
		url: `${config.MY_API_URL}/Role/UpdateRole`,
		name: "添加角色",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	deleteRoles: {
		url: `${config.MY_API_URL}/Role/DeleteRoles`,
		name: "删除角色",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
	roleAssignPermissions: {
		url: `${config.MY_API_URL}/Role/RoleAssignPermissions`,
		name: "删除角色",
		post: async function (data) {
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				},
			});
		},
	},
};
