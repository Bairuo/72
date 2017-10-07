# 使用说明

需挂载NetObject组件到所有包含同步变量组件的前面

为了兼容和性能测试等考虑，接口提供两种使用方法

为了保证网络畅通，应尽量满足以下要求：

* 协议发送频率66ms/次以下为宜（平均发送时间间隔≥66ms）

* 尽量在变量改变时才发送协议

## 1.自定义协议

```
    void foo()
    {
        NetObject netObject = GetComponent<NetObject>();
        
        ProtocolBytes proto = netObject.GetObjectProtocol();     // 初始协议必须使用netObject的方法获取
        
        netObject.AddOnceListener(注册名, CallBack);
        proto.AddString(注册名);                           // 协议名与注册名一致
        
        // 自定加入变量顺序和类型
        proto.AddInt...
        proto.AddFloat...
        
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
        
        ...
    }

```

```
    class A
    {
        NetObject NetObject;
    
        void Start()
        {
            netObject = GetComponent<NetObject>();
            
            netObject.AddListener(注册名, CallBack);
        }
        
        // 触发同步的事件
        void foo()
        {
            ProtocolBytes proto = netObject.GetObjectProtocol();
            
            proto.AddString(注册名);
            
            // 同例1
        }
        
        // 其他客户端接收到协议后的处理
        public void CallBack(ProtocolBase protocol)
        {
            // 同例1
        }
        
    }
```

## 2.自动同步变量SynInt, SynFloat, SynBool, SynString
