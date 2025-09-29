using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Iced.Intel.AssemblerRegisters;

namespace SoD2_Editor
{
    public partial class Form1 : Form
    {
        IntPtr AZDresults = IntPtr.Zero;
        int AZDresultsSize = 0x1000;
        IntPtr AZDstrings = IntPtr.Zero;
        int AZDstringsSize = 0x1000;
        private void btnHookZombieDamagedAnalytics_Click(object sender, EventArgs e)
        {
            //Hook Analytics for zombie hit
            IntPtr AnalyticsZombieDamagedHook = hooks.Get("AnalyticsZombieDamagedHook");
            IntPtr AnalyticsZombieDamagedReturn = hooks.Get("AnalyticsZombieDamagedReturn");
            IntPtr AZDhookedFunc = Alloc(0x1000);
            AZDresults = Alloc(AZDresultsSize);
            AZDstrings = Alloc(AZDstringsSize);
            int CauseOfDamageIdOffset = 0x80;
            int DealerStateOffset = 0xC0;
            int PreDamageStateOffset = 0x100;
            int ResultingStateOffset = 0x140;

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            //Start of hooked code
            asm.sub(rsp, 0x1000);
            asm.pushfq();



            //Copy AnalyticsHelper
            asm.mov(rax, AZDresults.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, rcx);
            asm.mov(rdi, rax);
            asm.mov(rcx, 0x200);
            asm.rep.movsb();
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);

            //Copy CauseOfDamageId String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x170]);
            asm.add(rax, CauseOfDamageIdOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x178]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x170], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy DealerState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x188]);
            asm.add(rax, DealerStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x190]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x188], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy PreDamageState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x1A8]);
            asm.add(rax, PreDamageStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x1B0]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x1A8], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);
            //Copy ResultingState String
            asm.mov(rax, AZDstrings.ToInt64());
            asm.push(rcx);
            asm.push(rsi);
            asm.push(rdi);
            asm.mov(rsi, __[rcx + 0x1B8]);
            asm.add(rax, ResultingStateOffset);
            asm.mov(rdi, rax);
            asm.mov(ecx, __[rcx + 0x1C0]);
            asm.add(ecx, ecx);
            asm.rep.movsb();
            asm.mov(rcx, AZDresults.ToInt64());
            asm.mov(__[rcx + 0x1B8], rax);
            asm.pop(rdi);
            asm.pop(rsi);
            asm.pop(rcx);








            asm.popfq();
            asm.add(rsp, 0x1000);
            //Replacing code we broke with our hook
            asm.push(rbp);
            asm.push(rbx);
            asm.push(rdi);
            asm.lea(rbp, rsp - 0x47);
            asm.sub(rsp, 0xa0);
            asm.mov(rax, AnalyticsZombieDamagedReturn.ToInt64());
            asm.jmp(rax);
            //Jmp back out of hook
            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)AZDhookedFunc);
            byte[] machineCode = stream.ToArray();
            WBytes(AZDhookedFunc, machineCode);


            asm = new Iced.Intel.Assembler(bitness: 64);
            asm.mov(rax, AZDhookedFunc.ToInt64());
            asm.jmp(rax);
            stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)AZDhookedFunc);
            machineCode = stream.ToArray();
            WBytes(AnalyticsZombieDamagedHook, machineCode);
            Output("Hooked ZombieDamageAnalytics");
        }

        private void btnUnhookZombieDamagedAnalytics_Click(object sender, EventArgs e)
        {

            IntPtr AnalyticsZombieDamagedHook = hooks.Get("AnalyticsZombieDamagedHook");

            Iced.Intel.Assembler asm = new Iced.Intel.Assembler(bitness: 64);
            asm.nop();
            asm.push(rbp);
            asm.push(rbx);
            asm.push(rdi);
            asm.lea(rbp, rsp - 0x47);
            asm.sub(rsp, 0xa0);
            var stream = new MemoryStream();
            asm.Assemble(new Iced.Intel.StreamCodeWriter(stream), (ulong)AnalyticsZombieDamagedHook);
            byte[] machineCode = stream.ToArray();
            WBytes(AnalyticsZombieDamagedHook, machineCode);


            Unalloc(AZDresults, 0x1000);
            Unalloc(AZDstrings, 0x1000);
            AZDresults = IntPtr.Zero;
            AZDstrings = IntPtr.Zero;
            Output("Unhooked ZombieDamageAnalytics");
        }
        private void UpdateZombieDamagedAnalytics()
        {

            if (!AZDresults.Equals(IntPtr.Zero))
            {
                var analytics = new ZombieDamagedAnalytics(AZDresults);

                lblAnalyticsZombieDamagedDetail.Text =
                    $"{"Zombie Type ID",-20}: {analytics.ZombieTypeId,7}\n" +
                    $"{"Cause of Damage",-20}: {analytics.CauseOfDamageType}\n" +
                    $"{"DealerState",-20}: {analytics.DealerState}\n" +
                    $"{"PreDamageState",-20}: {analytics.PreDamageState}\n" +
                    $"{"ResultingState",-20}: {analytics.ResultingState}\n" +
                    $"{"Zombie ID",-20}: {analytics.ZombieId,7}\n" +
                    $"{"Is Plague Zombie",-20}: {analytics.IsPlagueZombie,7}\n" +
                    $"{"PosX",-20}: {analytics.ZombieX,7}\n" +
                    $"{"PosY",-20}: {analytics.ZombieY,7}\n" +
                    $"{"PosZ",-20}: {analytics.ZombieZ,7}\n" +
                    $"{"Killed",-20}: {analytics.Killed,7}\n" +
                    $"{"Dealer ID",-20}: {analytics.DealerId,7}\n" +
                    $"{"Stun Chance",-20}: {analytics.StunChance,14:F6}\n" +
                    $"{"Down Chance",-20}: {analytics.DownChance,14:F6}\n" +
                    $"{"Kill Chance",-20}: {analytics.KillChance,14:F6}\n" +
                    $"{"Dismember Chance",-20}: {analytics.DismemberChance,14:F6}\n" +
                    $"{"Headshot Counter",-20}: {analytics.HeadshotCounter,7}";
            }
            else
                lblAnalyticsZombieDamagedDetail.Text = "Unhooked";
        }
    }
}
