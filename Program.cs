using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CW_TASK___Practice
{/*задание 4*/
    internal class Program
    {


        public class MyClass 
        { 

            const int CountNums = 50;
            public string File1 = "test1.txt";
            public string File2 = "test2.txt";
            public string File3 = "test3.txt";
            List<int> numberCollection;           //tempList


        public List<int> GenerateNumbers()      //генерируем список случайных чисел
        {
            numberCollection = new List<int>();
            Random random = new Random();

            for(int i = 0; i < CountNums; i++) 
            {
              numberCollection.Add(random.Next(0,100));
            }
            return numberCollection;
        }



        public List<int> FileReader(string path)        //считыванеи с файла в лист
        {
                lock(this)
                { 
                    
                    numberCollection = new List<int>();
                    FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader streamReader = new StreamReader(file);
                    try
                    {
                        int res;
                        while (int.TryParse(streamReader.ReadLine(), out res))
                        {
                            numberCollection.Add(res);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    file.Close();
                    streamReader.Close();
                    Console.WriteLine(path + "     FileRead_Ok");
                }
                return numberCollection;
        }



        public void FileWrite(string path , List<int> list)     //запись в файл
        {
                lock (this)
                {
                    try
                    {
                        FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter writer = new StreamWriter(file);
                        foreach (var el in list)
                        {
                            writer.WriteLine(el);
                        }
                        writer.Close();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine(path + "     FileWrite_Ok");
                }
        }

        private List<int> SelectPrimeNumsFromFile(List<int> listWithNums)  //выбираем из коллекции простые числа
            {
               
                List<int> res = new List<int>();
                foreach(var el in listWithNums)
                {
                    for(int i = 2; i < el; i++)
                    {
                        if (el % i == 0) break;
                        if( i==el-1) res.Add(el);
                    }
                }
              
                return res;
            }
            private List<int> SelectNumsEnding_7(List<int> listWithNums)//выбираем из коллекции простых чисел те , которые % 10==7
            {

                List<int> res = new List<int>();
                foreach (var el in listWithNums)
                {
                   if(el%10 == 7) res.Add(el);
                }
                foreach (var el in res)
                    Console.WriteLine(el);
                return res;
            }


            public void WriteFileWithRandomNums()       //записываем рандомные числа в 1 файл
        {
            numberCollection = GenerateNumbers();
            FileWrite(File1, numberCollection);
        }

        public void WriteFileWithSimpleNums()  // записываем простые числа во 2 файл считывая с первого файла
        {
            FileWrite(File2, SelectPrimeNumsFromFile(FileReader(File1)));
        }

        public void WriteFileWithNumsEnding_7()
            {
                FileWrite(File3, SelectNumsEnding_7(FileReader(File2)));
            }

 }

        static void ThreadFoo1()
        {
            MyClass myClass = new MyClass();

            try
            {
                Mutex mutex = new Mutex(false, "B58A4FBE-0F50-4180-977E-7126DFEC8B23");
                mutex.WaitOne();
                myClass.WriteFileWithRandomNums();
                mutex.ReleaseMutex();

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message); 
            }
        }
        static void ThreadFoo2()
        {
            MyClass myClass = new MyClass();

            try
            {
                Mutex mutex = new Mutex(false, "B58A4FBE-0F50-4180-977E-7126DFEC8B23");
                mutex.WaitOne();
                myClass.WriteFileWithSimpleNums();
                mutex.ReleaseMutex();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void ThreadFoo3()
        {
            MyClass myClass = new MyClass();

            try
            {
                Mutex mutex = new Mutex(false, "B58A4FBE-0F50-4180-977E-7126DFEC8B23");
                mutex.WaitOne();
                myClass.WriteFileWithNumsEnding_7();
                mutex.ReleaseMutex();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }





        static void Report()
        {

        }

        static void Main(string[] args)
        {
           
            MyClass myClass = new MyClass();

         

            Task task1 = new Task(ThreadFoo1);
            Task task2 = new Task(ThreadFoo2);
            Task task3 = new Task(ThreadFoo3);

            task1.Start();
            task2.Start();
            task3.Start();

            Task.WaitAll(task1,task2,task3);

        }
    }
}
