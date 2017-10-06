# 使用说明

需挂载NetObject组件到所有包含同步变量组件的前面

为了兼容和性能测试等考虑，接口提供两种使用方法

为了保证网络畅通，应尽量满足以下要求：

* 协议发送频率66ms/次以下为宜（平均发送时间间隔≥66ms）

## 1自定义协议

```
    void foo()
    {
        NetObject netObject = GetComponent<NetObject>();
        
        ProtocolBytes proto = netObject.GetObjectProtocol();     // 初始协议必须使用netObject的方法获取
        
        netObject.AddOnceListener(注册名, CallBack);
        proto.AddString(CallBackName);                           // 协议名与注册名一致
        
        // 自定加入变量顺序和类型
        proto.AddInt...
        proto.AddFloat...
        
        ...
        
        netObject.Send(proto);
    }
    
    // 回调函数
    
    public void CallBack(ProtocolBase protocol)
    {
        string name = proto.GetProtoName();
        
        // 自定义的顺序和类型
        int a = proto.GetInt();
        int b = proto.GetFloat();
        
        ...
    }

```

## 2自动同步变量SynInt, SynFloat, SynBool, SynString
