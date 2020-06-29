namespace EdugameCloud.Lti.GetHashCodeTrick
{
    public static class SakaiCourseNumberTrick
    {
        //static void Main(string[] args)
        //{
        //    string val = "ХЕР";
        //    int val1 = GetHashCode(val);
        //    //int val2 = GetHashCode2(val);

        //    int val3 = val.GetHashCode();
        //    //1767037370
        //    // 596230372
        //    Console.WriteLine("Hello World!");
        //}


        //public static unsafe int GetHashCode2(string value)
        //{
        //    fixed (char* str = value)
        //    {
        //        char* chPtr = str;
        //        int num = 352654597;
        //        int num2 = num;
        //        int* numPtr = (int*)chPtr;
        //        for (int i = value.Length; i > 0; i -= 4)
        //        {
        //            num = (((num << 5) + num) + (num >> 27)) ^ numPtr[0];
        //            if (i <= 2)
        //            {
        //                break;
        //            }
        //            num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ numPtr[1];
        //            numPtr += 2;
        //        }
        //        return (num + (num2 * 1566083941));
        //    }
        //}


        public static int GetHashCode(string value)
        {

//#if FEATURE_RANDOMIZED_STRING_HASHING
//            if(HashHelpers.s_UseRandomizedStringHashing)
//            { 
//                return InternalMarvin32HashString(this, this.Length, 0);
//            } 
//#endif // FEATURE_RANDOMIZED_STRING_HASHING 

            unsafe
            {
                fixed (char* src = value)
                {
                    //Contract.Assert(src[value.Length] == '\0', "src[this.Length] == '\\0'");
                    //Contract.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

//#if WIN32
//                    int hash1 = (5381<<16) + 5381; 
//#else 
                    int hash1 = 5381;
//#endif 
                    int hash2 = hash1;

//#if WIN32
//                    // 32 bit machines. 
//                    int* pint = (int *)src;
//                    int len = this.Length; 
//                    while (len > 2) 
//                    {
//                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0]; 
//                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
//                        pint += 2;
//                        len  -= 4;
//                    } 

//                    if (len > 0) 
//                    { 
//                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
//                    } 
//#else
                    int c;
                    char* s = src;
                    while ((c = s[0]) != 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        c = s[1];
                        if (c == 0)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                        s += 2;
                    }
//#endif
//#if DEBUG 
//                    // We want to ensure we can change our hash function daily.
//                    // This is perfectly fine as long as you don't persist the 
//                    // value from GetHashCode to disk or count on String A 
//                    // hashing before string B.  Those are bugs in your code.
//                    hash1 ^= ThisAssembly.DailyBuildNumber;
//#endif
                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

    }

}