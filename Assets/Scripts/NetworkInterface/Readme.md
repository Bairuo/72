# ʹ��˵��

�����NetObject��������а���ͬ�����������ǰ��

Ϊ�˼��ݺ����ܲ��Եȿ��ǣ��ӿ��ṩ����ʹ�÷���

Ϊ�˱�֤���糩ͨ��Ӧ������������Ҫ��

* Э�鷢��Ƶ��66ms/������Ϊ�ˣ�ƽ������ʱ������66ms��

## 1�Զ���Э��

```
    void foo()
    {
        NetObject netObject = GetComponent<NetObject>();
        
        ProtocolBytes proto = netObject.GetObjectProtocol();     // ��ʼЭ�����ʹ��netObject�ķ�����ȡ
        
        netObject.AddOnceListener(ע����, CallBack);
        proto.AddString(CallBackName);                           // Э������ע����һ��
        
        // �Զ��������˳�������
        proto.AddInt...
        proto.AddFloat...
        
        ...
        
        netObject.Send(proto);
    }
    
    // �ص�����
    
    public void CallBack(ProtocolBase protocol)
    {
        string name = proto.GetProtoName();
        
        // �Զ����˳�������
        int a = proto.GetInt();
        int b = proto.GetFloat();
        
        ...
    }

```

## 2�Զ�ͬ������SynInt, SynFloat, SynBool, SynString
