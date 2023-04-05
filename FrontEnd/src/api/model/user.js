import config from "@/config"
import http from "@/utils/request"

export default {
	list: {
		url: `${config.MY_API_URL}/User/List`,
		name: "获取用户",
		get: async function(){
			return await http.get(this.url);
		}
	},
	
	getUsersByPage: {
		url: `${config.MY_API_URL}/User/GetUsersByPage`,
		name: "添加用户",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
	addUser: {
		url: `${config.MY_API_URL}/User/AddUser`,
		name: "添加用户",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
	updateUser: {
		url: `${config.MY_API_URL}/User/UpdateUser`,
		name: "添加用户",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
	deleteUsers: {
		url: `${config.MY_API_URL}/User/DeleteUsers`,
		name: "删除用户",
		post: async function(data){
			return await http.post(this.url, data, {
				headers: {
					//'response-status': 401
				}
			});
		}
	},
}
