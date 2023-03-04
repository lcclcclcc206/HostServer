## 参考

https://learn.microsoft.com/zh-cn/iis/get-started/getting-started-with-iis/getting-started-with-appcmdexe

## 将 AppCmd 的目录添加至系统环境变量

AppCmd 的目录位于 `C:\Windows\System32\inetsrv`，将其添加至系统环境变量，以方便直接调用。

或者，直接在命令行中临时设置：

**cmd**

```cmd
set PATH=C:\Windows\System32\inetsrv;%PATH%
```

**PowerShell**

```powershell
$Env:Path="C:\Windows\System32\inetsrv;$Env:Path"
```

## 管理 IIS 网站

**列出虚拟站点**

```cmd
shell> appcmd list site
SITE "HostServer" (id:1,bindings:http/*:80:,state:Unknown)

shell> appcmd list site "HostServer"
SITE "HostServer" (id:1,bindings:http/*:80:,state:Unknown)
```

**列出虚拟站点物理地址**

```cmd
shell> appcmd list vdir
VDIR "HostServer/" (physicalPath:D:\HostServer)
```

**备份 IIS 全局配置**

配置文件位于 `C:\Windows\System32\inetsrv\backup`

```cmd
shell> appcmd add backup
已添加 BACKUP 对象“20230228T231935”
```

**停止与启动站点（需要管理员权限）**

```cmd
shell> gsudo # 提升管理员权限
shell> appcmd stop site "HostServer"
“HostServer”已成功停止

shell> appcmd start site "HostServer"
“HostServer”已成功启动。
```

> gsudo 请自行下载安装

> ```cmd
> # 重启站点
> appcmd stop site "HostServer"
> appcmd start site "HostServer"
> ```