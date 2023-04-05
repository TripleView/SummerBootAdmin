import config from "@/config"
import http from "@/utils/request"

export default {
	tree: {
		url: `${config.MY_API_URL}/Department/List`,
		name: "获取部门",
		get: async function(){
			return await http.get(this.url);
		}
	},
	addDepartment: {
		url: `${config.MY_API_URL}/Department/AddDepartment`,
		name: "添加菜单",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
	updateDepartment: {
		url: `${config.MY_API_URL}/Department/UpdateDepartment`,
		name: "添加菜单",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
	deleteDepartments: {
		url: `${config.MY_API_URL}/Department/DeleteDepartments`,
		name: "删除菜单",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
}
