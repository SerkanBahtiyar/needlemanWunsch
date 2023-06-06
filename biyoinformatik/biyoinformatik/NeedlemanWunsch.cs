using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace biyoinformatik
{
    internal class NeedlemanWunsch
    {
         int Match { get; set; }
         int Gap { get; set; }
         int Mismatch { get; set; }
        // sütün sequence
         string birinciSequence { get; set; }
         string ikinciSequence { get; set; }
         int[,] Matrix { get; set; }
        // satır sequence
        // olası back traceler
        public List<AlignedSequencePair> AlignedSequencePairs { get; set; }
        public List<List<Trace>> BackTraces { get; set; }
        

        public NeedlemanWunsch(int match, int mismatch, int gap)
        {
            Match = match;
            Mismatch = mismatch;
            Gap = gap;
            BackTraces = new List<List<Trace>>();
            AlignedSequencePairs = new List<AlignedSequencePair>();
            dosyalarıOku();
        }
        public NeedlemanWunsch(int match, int mismatch, int gap, string firstSequence, string secondSequence)
        {
            Match = match;
            Mismatch = mismatch;
            Gap = gap;
            birinciSequence = firstSequence;
            ikinciSequence = secondSequence;
            Matrix = new int[firstSequence.Length + 1, secondSequence.Length + 1];
            BackTraces = new List<List<Trace>>();
            AlignedSequencePairs = new List<AlignedSequencePair>();
        }

        public void dosyalarıOku()
        {
          
            string seq2 = @"C:\\Users\\serka\\OneDrive\\Masaüstü\\ser2.txt";
            string okunanMetin2 = "";
            if (File.Exists(seq2))
            {
                okunanMetin2 = File.ReadAllText(seq2);
                ikinciSequence = okunanMetin2;
            }
            else
            {
                Console.WriteLine("bulunmadı");
            }

           //okuma işlemi için iki farklı şekilde okuma gerçekleştirildi
            string dosya_yolu = @"C:\\Users\\serka\\OneDrive\\Masaüstü\\ser.txt";

            //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);

            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosyanın açılacağını,
            //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
            StreamReader sw = new StreamReader(fs);

            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            string FirstSequenceM = sw.ReadLine();
            birinciSequence = FirstSequenceM;
            sw.Close();
            fs.Close();
            Matrix = new int[birinciSequence.Length + 1, ikinciSequence.Length + 1];
        }

        //matrisi doldurma
        public void MatrisDoldur()
        {   //ilk satır ve sütünü gap değeri ile doldur
           
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                Matrix[i, 0] = i * Gap;
            }

            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                Matrix[0, j] = j * Gap;
            }

            // matris çaprazla ve değerleri hesapla

            for (int i = 1; i < Matrix.GetLength(0); i++)
            {
                for (int j = 1; j < Matrix.GetLength(1); j++)
                {
                    int leftValue = Matrix[i, j - 1] + Gap;
                    int topValue = Matrix[i - 1, j] + Gap;
                    int diagonalValue = Matrix[i - 1, j - 1] +
                                        (birinciSequence[i - 1] == ikinciSequence[j - 1] ? Match : Mismatch);
                    Matrix[i, j] = Math.Max(Math.Max(topValue, leftValue), diagonalValue);
                }
            }
        }

        public void TraceBack(List<Trace> traces)
        {
            // devam tracing kadar Matrix[1,1], Matrix[1,0] or Matrix[0,1]
            while (!((traces.Last().RowIndex == 1 && traces.Last().ColIndex == 1) ||
                     (traces.Last().RowIndex == 1 && traces.Last().ColIndex == 0) ||
                     (traces.Last().RowIndex == 0 && traces.Last().ColIndex == 1)))
            {
                bool isSourceTop = false;
                bool isSourceLeft = false;
                bool isSourceDiagonal = false;

                int topValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex] + Gap;
                int leftValue = Matrix[traces.Last().RowIndex, traces.Last().ColIndex - 1] + Gap;
                int diagonalValue = Matrix[traces.Last().RowIndex - 1, traces.Last().ColIndex - 1] +
                                    (birinciSequence[traces.Last().RowIndex - 1] ==
                                     ikinciSequence[traces.Last().ColIndex - 1]
                                        ? Match
                                        : Mismatch);
                if (leftValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceLeft = true;
                }
                if (diagonalValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceDiagonal = true;
                }
                if (topValue == Matrix[traces.Last().RowIndex, traces.Last().ColIndex])
                {
                    isSourceTop = true;
                }
                if (isSourceTop && isSourceLeft && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);

                    //üst koşul
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace); 

                    
                    tempTrace = new List<Trace>(traces);
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                    TraceBack(tempTrace);
                    traces.Add(new Trace
                    {
                        ColIndex = traces.Last().ColIndex - 1,
                        RowIndex = traces.Last().RowIndex - 1,
                        
                    });
                }
                else if (isSourceTop && isSourceLeft)
                {
                    var tempTrace = new List<Trace>(traces);

                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace);

                    
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceTop && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);

                    
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                    TraceBack(tempTrace);

                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceLeft && isSourceDiagonal)
                {
                    var tempTrace = new List<Trace>(traces);

                    
                    tempTrace.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                    TraceBack(tempTrace);

                    
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceLeft)
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
                else if (isSourceTop)
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex
                    });
                }
              
                else
                {
                    traces.Add(new Trace
                    {
                        RowIndex = traces.Last().RowIndex - 1,
                        ColIndex = traces.Last().ColIndex - 1
                    });
                }
            }

            traces.Add(new Trace { RowIndex = 0, ColIndex = 0 });
            BackTraces.Add(traces);
        }

 
        /// back tracelere göre muhtemel sequenceleri bul
         public void SequencesHizalama()
        {
            for (int i = 0; i < BackTraces.Count; i++)
            {
                string firstAlignedSequence = "";
                string secondAlignedSequence = "";
                for (int j = 0; j < BackTraces[i].Count - 1; j++)
                {
                    if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                        BackTraces[i][j].ColIndex - 1 == BackTraces[i][j + 1].ColIndex)
                    {
                        firstAlignedSequence += birinciSequence[BackTraces[i][j].RowIndex - 1];
                        secondAlignedSequence += ikinciSequence[BackTraces[i][j].ColIndex - 1];
                    }
                    else if (BackTraces[i][j].RowIndex - 1 == BackTraces[i][j + 1].RowIndex &&
                             BackTraces[i][j].ColIndex == BackTraces[i][j + 1].ColIndex)
                    {
                        firstAlignedSequence += birinciSequence[BackTraces[i][j].RowIndex - 1];
                        secondAlignedSequence += "-";
                    }
                    else
                    {
                        firstAlignedSequence += "-";
                        secondAlignedSequence += ikinciSequence[BackTraces[i][j].ColIndex - 1];
                    }
                }

                AlignedSequencePairs.Add(new AlignedSequencePair
                {
                    FirstAlignedSequence = firstAlignedSequence.Reverse(),
                    SecondAlignedSequence = secondAlignedSequence.Reverse()
                });
            }
        }

       //değerleri yazma
        public void Yazdir()
        {
             Console.WriteLine("-hesaplanmış matrix-");
            Console.WriteLine("\n");
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                if (i == 0)
                {
                    Console.Write("\t\t");
                    
                    for (int j = 0; j < Matrix.GetLength(1); j++)
                    {
                        if (j != 0)
                        {
                            Console.Write("*");
                            Console.Write(ikinciSequence[j - 1] + "\t");
                            
                        }
                        
                    }

                    
                    Console.WriteLine();
                   
                }

                if (i != 0)
                {
                    
                    Console.Write(birinciSequence[i - 1]);
                    
                }
                
                Console.Write("\t");
               

                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Console.Write(Matrix[i, j] + "\t");
                }

                Console.WriteLine();
            }

            // puan
            Console.WriteLine("\n");
            Console.WriteLine("Puan değeri> " + Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1]);
            // optimal hizalamalar
            Console.WriteLine("\n\noptimal global hizalanması;");

            foreach (var tara in AlignedSequencePairs)
            {
                 Console.WriteLine(tara.FirstAlignedSequence);
                Console.WriteLine(tara.SecondAlignedSequence);
            }

            
        }

         /// sequence bilgilerine göre hizalama işlemini çalıştırma
        
        public void algoritma()
        {
            var sure = System.Diagnostics.Stopwatch.StartNew();
            MatrisDoldur();
            var traces = new List<Trace>();
            traces.Add(new Trace
            {
                RowIndex = Matrix.GetLength(0) - 1,
                ColIndex = Matrix.GetLength(1) - 1
            });
            TraceBack(traces);
            SequencesHizalama();
            sure.Stop();

            Yazdir();
            Console.WriteLine("\n");
            Console.WriteLine("Çalışma süresi " + sure.ElapsedMilliseconds + " milisaniye");
        }
    }


    public class AlignedSequencePair
    {
        public string FirstAlignedSequence { get; set; }
        public string SecondAlignedSequence { get; set; }
    }

    public class Trace
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
    }
}
    
