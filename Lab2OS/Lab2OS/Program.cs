using System;

namespace Lab2OS
{
    class Program
    {
        static void test_alloc(Allocator allocator, out int first_address, out int second_address)
        {
            allocator.mem_dump();
            int addr = allocator.mem_alloc(100);
            Console.WriteLine($"Block address 1: \t {addr}");
            allocator.block_filling(addr);
            allocator.mem_dump();
            int addr2 = allocator.mem_alloc(30);
            Console.WriteLine($"Block address 2: \t {addr2}");
            allocator.block_filling(addr2);
            allocator.mem_dump();
            first_address = addr;
            second_address = addr2;
        }

        static void test_mem_free(Allocator allocator, int addr)
        {
            allocator.mem_free(addr);
            allocator.mem_dump();
        }

        static void test_mem_realloc(Allocator allocator)
        {
            int addr = allocator.mem_alloc(200);
            allocator.block_filling(addr);
            allocator.mem_dump();
            allocator.mem_realloc(addr, 20);
            try
            {
                allocator.mem_free(addr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Memory reallocated, cannot free by old address");
            }
        }

        static void Main(string[] args)
        {
            Allocator allocator = new Allocator(20, 30);

            test_alloc(allocator, out int first_addr, out int second_address);
            test_mem_free(allocator, first_addr);
            test_mem_realloc(allocator);
            Console.ReadLine();

        }
    }
}
