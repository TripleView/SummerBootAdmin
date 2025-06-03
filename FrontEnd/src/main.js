import { createApp } from "vue";
import ElementPlus from "element-plus";
import "element-plus/dist/index.css";
import "element-plus/theme-chalk/display.css";
import scui from "./scui";
import i18n from "./locales";
import router from "./router";
import store from "./store";
import App from "./App.vue";

const app = createApp(App);

app.use(store);
app.use(router);
app.use(ElementPlus);
app.use(i18n);
app.use(scui);

app.config.globalProperties.$clone = function (obj) {
	return JSON.parse(JSON.stringify(obj));
};

app.config.globalProperties.$localDateFormat = function (utcString) {
	if (!utcString) return "";
	const date = new Date(utcString);
	return date.toLocaleString();
};

app.config.globalProperties.$getDictionaryItems = async function (code) {
	var res =
		await app.config.globalProperties.$API.system.dic.getDictionaryItems.get(
			code
		);
	if (res.code == 20000) {
		return res.data;
	}
	return [];
};

// app.vue写在script里面  main.js写在app挂载完之后
const debounce = (fn, delay) => {
	let timer;
	return (...args) => {
		if (timer) {
			clearTimeout(timer);
		}
		timer = setTimeout(() => {
			fn(...args);
		}, delay);
	};
};
const _ResizeObserver = window.ResizeObserver;
window.ResizeObserver = class ResizeObserver extends _ResizeObserver {
	constructor(callback) {
		callback = debounce(callback, 200);
		super(callback);
	}
};

//挂载app
app.mount("#app");
