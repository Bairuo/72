# ʹ��˵��

�����NetObject��������а���ͬ�����������ǰ��

Ϊ�˼��ݺ����ܲ��Եȿ��ǣ��ӿ��ṩ����ʹ�÷���

Ϊ�˱�֤���糩ͨ��Ӧ������������Ҫ��

* Э�鷢��Ƶ��66ms/������Ϊ�ˣ�ƽ������ʱ������66ms��

* �����ڱ����ı�ʱ�ŷ�����Ϣ

## 1.�Զ���Э��


```
    class A
    {
        NetObject NetObject;
        float Health;
        int Damage;
    
        void Start()
        {
            netObject = GetComponent<NetObject>();
            
            // Ӧ��֤���еĻص�������ǰע��
            // ������Start�������
            netObject.AddListener(ע����, CallBack);
        }
        
        // �����Է����ı䣬��Ҫ����Э�鵽�����ͻ������ͬ��
        void foo()
        {
            ProtocolBytes proto = netObject.GetObjectProtocol();
            
            // �����ȼ���Э��������ע��������һ��
            proto.AddName(ע����);
            
            // �Զ��������˳�������
            proto.AddInt...
            proto.AddFloat...
            proto.AddString...
            
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
            string c = proto.GetString();
            
            ...

        }
        
    }
```

## 2.�Զ�ͬ������SynInt, SynFloat, SynBool, SynString
