﻿using System.ComponentModel;

namespace SummerBootAdmin.Dto.User;

public class AddUserDto
{
    [Description("账号")]
    public string Account { get; set; }
    [Description("姓名")]
    public string Name { get; set; }
   
    [Description("部门id")]
    public int DepartmentId { get; set; }
    [Description("密码")]
    public string Password { get; set; }
    [Description("头像")]
    public string Avatar { get; set; }
    /// <summary>
    /// 角色id列表
    /// </summary>
    public List<int> RoleIds { get; set; }
}