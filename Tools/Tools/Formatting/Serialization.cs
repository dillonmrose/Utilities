using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

public class Serialization
{
    public static void Run()
    {

        //Creates a new TestSimpleObject object.
        List<TestSimpleObject> obj = new List<TestSimpleObject>();
        obj.Add(new TestSimpleObject());
        obj.Add(new TestSimpleObject());

        Console.WriteLine("Before serialization the object contains: ");
        foreach (TestSimpleObject ob in obj)
        {
            ob.Print();
        }
    

        //Opens a file and serializes the object into it in binary format.
        Stream stream = File.Open("data.xml", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();

        //BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, obj);
        stream.Close();

        //Empties obj.
        obj = null;

        //Opens file "data.xml" and deserializes the object from it.
        stream = File.Open("data.xml", FileMode.Open);
        formatter = new BinaryFormatter();

        //formatter = new BinaryFormatter();

        obj = (List<TestSimpleObject>)formatter.Deserialize(stream);
        stream.Close();

        Console.WriteLine("");
        Console.WriteLine("After deserialization the object contains: ");
        foreach (TestSimpleObject ob in obj)
        {
            ob.Print();
        }
        Console.ReadLine();
    }
}


// A test object that needs to be serialized.
[Serializable()]
public class TestSimpleObject
{

    public List<String> list;
    public HashSet<String> set;
    public string [] array;
    public string member;

    public TestSimpleObject()
    {
        list = new List<string>();
        list.Add("list1");
        list.Add("list2");
        set = new HashSet<string>();
        set.Add("set1");
        set.Add("set2");
        array = new string[2];
        array[0] = "array1";
        array[1] = "array2";
        member = "hello world!";
    }


    public void Print()
    {

        Console.WriteLine("list =");
        foreach(string element in list)
        {
            Console.WriteLine(element);
        }
        Console.WriteLine("set = ");
        foreach (string element in set)
        {
            Console.WriteLine(element);
        }
        Console.WriteLine("array = ");
        foreach (string element in array)
        {
            Console.WriteLine(element);
        }
        Console.WriteLine("member = '{0}'", member);
    }
}