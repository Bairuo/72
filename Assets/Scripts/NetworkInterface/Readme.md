# ʹ��˵��

�����NetObject��������а���ͬ�����������ǰ��

Ϊ�˼��ݺ����ܲ��Եȿ��ǣ��ӿ��ṩ����ʹ�÷���

Ϊ�˱�֤���糩ͨ��Ӧ������������Ҫ��

* Э�鷢��Ƶ��66ms/������Ϊ�ˣ�ƽ������ʱ������66ms��

* �����ڱ����ı�ʱ�ŷ���Э��

## 1.�Զ���Э��

```
    void foo()
    {
        NetObject netObject = GetComponent<NetObject>();
        
        ProtocolBytes proto = netObject.GetObjectProtocol();     // ��ʼЭ�����ʹ��netObject�ķ�����ȡ
        
        netObject.AddOnceListener(ע����, CallBack);
        proto.AddString(ע����);                           // Э������ע����һ��
        
        // �Զ��������˳�������
        proto.AddInt...
        proto.AddFloat...
        
        ...
        
        // ����
        netObject.Send(proto);
    }
    
    // �����ͻ��˽��յ�Э���Ĵ���
    public void CallBack(ProtocolBase protocol)
    {
        //ʹ�ò����ε�GetInt�ȱ���Ҫ�ȵ���GetName()
        string name = proto.GetName();
        
        // �Զ����˳�������
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
            
            netObject.AddListener(ע����, CallBack);
        }
        
        // ����ͬ�����¼�
        void foo()
        {
            ProtocolBytes proto = netObject.GetObjectProtocol();
            
            proto.AddString(ע����);
            
            // ͬ��1
        }
        
        // �����ͻ��˽��յ�Э���Ĵ���
        public void CallBack(ProtocolBase protocol)
        {
            // ͬ��1
        }
        
    }
```

## 2.�Զ�ͬ������SynInt, SynFloat, SynBool, SynString
