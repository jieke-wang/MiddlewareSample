using System;

namespace MiddlewareSample.Middlewares.Options
{
    public class MyOptions
    {
        internal const string OptionsKey = "MyOptions";

        public string Name { get; set; } = "jack 来自默认值";
        public int Age { get; set; } = default;
        public string[] Hobbies { get; set; } = { "Hobby1", "Hobby2", "Hobby2" };
    }
}