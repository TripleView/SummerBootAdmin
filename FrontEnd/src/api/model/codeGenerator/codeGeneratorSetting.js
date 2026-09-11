import config from "@/config";
import http from "@/utils/request";

export default {
	treeList: {
		url: `${config.MY_API_URL}/codeGeneratorSetting/TreeList`,
		name: "获取用户",
		get: async function () {
			return await http.get(this.url);
		},
	},
};
