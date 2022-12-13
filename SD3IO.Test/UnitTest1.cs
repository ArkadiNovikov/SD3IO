namespace SD3IO.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var path = @"save.ram";
            var path2 = @"save.b.ram";

            SD3IO io = new SD3IO();
            var data = io.Read(path);

            var name = data.slots[0].data.characterName1;
            var sname = io.NameByteArrayToString(name);
            io.Write(path2, data);
        }

        [Fact]
        public void TestGetData1()
        {
            var i = new JSON.Characters();
            Data.GetData<JSON.Characters>(ref i);
            
        }
    }
}