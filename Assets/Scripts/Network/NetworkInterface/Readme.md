# 使用说明

需挂载NetObject组件到所有包含同步变量组件的前面

为了兼容和性能测试等考虑，接口提供两种使用方法

为了保证网络畅通，应尽量满足以下要求：

* 协议发送频率66ms/次以下为宜（平均发送时间间隔≥66ms）

* 尽量在变量改变时才发送消息

## 1.自定义协议


```
    class A
    {
        NetObject NetObject;
        float Health;
        int Damage;
    
        void Start()
        {
            netObject = GetComponent<NetObject>();
            
            // 应保证所有的回调都已提前注册
            // 建议在Start里面完成
            netObject.AddListener(注册名, CallBack);
        }
        
        // 有属性发生改变，需要发送协议到其它客户端完成同步
        void foo()
        {
            ProtocolBytes proto = netObject.GetObjectProtocol();
            
            // 需首先加入协议名，与注册名保持一致
            proto.AddName(注册名);
            
            // 自定加入变量顺序和类型
            proto.AddInt...
            proto.AddFloat...
            proto.AddString...
            
            ...
            
            // 发送
            netObject.Send(proto);
            
        }
        
        // 其他客户端接收到协议后的处理
        public void CallBack(ProtocolBase protocol)
        {
            //使用不带参的GetInt等必须要先调用GetName()
            string name = proto.GetName();
        
            // 自定义的顺序和类型
            int a = proto.GetInt();
            int b = proto.GetFloat();
            string c = proto.GetString();
            
            ...

        }
        
    }
```

## 2.自动同步变量SynInt, SynFloat, SynBool, SynString
