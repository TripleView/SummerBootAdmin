const files = require.context("./model", true, /\.js$/);

const modules = {};

files.keys().forEach((key) => {
	const modulePath = key.replace(/^\.\/|\.js$/g, "").split("/");

	const moduleName = modulePath.pop();

	let current = modules;

	modulePath.forEach((directory) => {
		if (!current[directory]) {
			current[directory] = {};
		}

		current = current[directory];
	});

	current[moduleName] = files(key).default;
});

export default modules;
