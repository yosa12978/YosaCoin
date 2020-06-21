using System;

namespace YosaCoin
{
    public class Draw
    {
        public static void draw_menu()
        {
            draw_logo();
            Console.WriteLine($"\nWelcome {Program.username}");
            Console.WriteLine($"Balance: {Program.yosaCoin.GetBalance(Program.username)} YosaCoin's\n");
            Console.Write("\n[MAIN MENU]\n1.Get Blockchain\n2.Create Transaction\n3.Mine Coins\n4.Exit\n");
        }

        public static void draw_logo()
        {
            Console.Clear();
            Console.Title = "YosaCoin CLI";
            ConsoleColor classic_console_color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ll      ll   llllllll    lllllll     llllllll    ||    llllllll   lllllllll   ll  ll      ll ");
            Console.WriteLine("   ll    ll   ll      ll  ll          ll      ll   ||   ll         ll       ll      llll    ll ");
            Console.WriteLine("    ll  ll    ll      ll  ll          ll      ll   ||   ll         ll       ll  ll  ll ll   ll ");
            Console.WriteLine("     llll     ll      ll   llllllll   llllllllll   ||   ll         ll       ll  ll  ll  ll  ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll   ll ll ");
            Console.WriteLine("      ll      ll      ll          ll  ll      ll   ||   ll         ll       ll  ll  ll    llll ");
            Console.WriteLine("      ll       llllllll    llllllll   ll      ll   ||    llllllll   lllllllll   ll  ll     lll\n ");
            Console.ForegroundColor = classic_console_color;
        } 
    } 
}