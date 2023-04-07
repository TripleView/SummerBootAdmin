[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/dotnetcore/CAP/master/LICENSE.txt)

# 核心理念
SummerBootAdmin,致力于打造一个易上手，好维护的人性化框架，让大家早点下班去做自己喜欢的事。

# 框架说明
这是一个基于前后端分离的通用后端管理框架，声明式编程，开发简单，易于维护。

# Getting Started
前后端分离架构，所以要分成2部分分别打开

## 后端
后端底层框架为.net6，所以使用vs2022直接打开解决方案即可，开发框架为SummerBoot，同时本项目也是[SummerBoot](https://github.com/TripleView/SummerBoot)的示例项目，文档可参考[SummerBoot](https://github.com/TripleView/SummerBoot)，
### 数据库
项目中采用的是mysql数据库(因为SummerBoot支持sqlserver，mysql，oracle，sqlite,pgsql等5种数据库，所以大家也可以选择自己喜欢的数据库),在mysql里添加一个数据库，然后修改项目中配置文件appsettings.json/appsettings.Development.json里的数据库连接字符串mysqlDbConnectionString为自己的数据库连接字符串，我这里添加的数据库名字是summerboot，所以数据库连接字符串为User Id=root;Password=123456;Data Source=localhost;database=summerboot;AllowLoadLocalInfile=true
### 初始化
运行项目，访问http://localhost:5099/swagger/index.html
在swagger里访问​/api​/GeneraterTable​/Index方法生成数据库，后端就算初始化完成了。

## 前端
前端部分基于同为MIT协议的开源项目[SCUI](https://gitee.com/lolicode/scui)，技术栈为Vue3 + Element-Plus,文档请参考[SCUI](https://lolicode.gitee.io/scui-doc/guide/)
### 安装教程
``` sh
# 进入前端目录
cd FrontEnd

# 安装依赖
npm i

# 启动项目(开发模式)
npm run serve

# 生成发布文件
# npm run build
```
启动完成后浏览器访问 http://localhost:5098


# 加入QQ群反馈建议
群号:799648362
