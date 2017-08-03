使用示例见：SceneManager.cs

# Debug模式，连接自己，去掉注释
//ServerNet.instance.Start("127.0.0.1", 9970);

# 每次联机启动一次
启动 new Client()

# 连接服务器
Client.instance.Connect()

# 调用网络消息处理
Client.instance.Update()