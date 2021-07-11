本示例展示了asp.net core中间件的使用，结合配置，针对普通中间件的各式配置花样注入，替换中间件工厂配合IMiddleware类型中间件扩展；

IMiddleware类型中间件实现了请求流和响应流的可重复读写，并对响应流做了gzip压缩处理；

本实例展示如下效果：

- Extentions.cs
  - 封装了服务注册
    - 注册`IMiddleware`类型中间件
    - 替换`IMiddlewareFactory`
  - 封装了配置
    - 添加Options
    - 绑定配置文件中的配置
    - Configure Action式的自定义配置
  - 封装了中间件管道的组装过程
- CompressMiddleware.cs
  - IMiddleware类型中间件示例
  - 资源可释放
  - 请求流和响应流的重复读写
  - 对请求和响应的日志记录
  - 对响应的gzip压缩

- MyMiddlewareFactory.cs
  - 替换原有中间件工厂
  - 实例化中间件
  - 释放中间件资源

- MyMiddleware1.cs
  - 中间件的基本用法
- MyMiddleware2.cs
  - 中间件的依赖注入用法
- MyMiddleware3.cs
  - 中间件的依赖注入
  - 中间件的配置注入

- MyMiddleware4.cs
  - 中间件的依赖注入
  - 中间件的自定义参数
- MyMiddleware5.cs
  - 中间件的依赖注入
  - 中间件的自定义参数
  - 中间件的自定义参数结合配置依赖注入的Configure Action



总结:

- options的优先级为: Configure Action > 配置文件 > 默认值, 如果配置中的属性为集合,会自动合并默认值和配置,且默认值在前
- 当中间件提供参数时,优先使用传递的参数,其次是默认值
- 当中间件参数结合配置注入时,优先使用Configure Action,其次是中间件参数,再次是默认值,不会使用配置文件中的配置
- 中间件的执行方式为按俄罗斯套娃的嵌套过程执行,即 以1->2->3->2->1的顺序执行,且配置在越前面的中间件会越早调用,越晚结束执行
- 每层中间件均可以在下一个中间件前后执行自定义的AOP代码,且随时可以中断调用过程,直接返回

