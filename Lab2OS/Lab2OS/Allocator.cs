using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2OS
{
    class Allocator
    {
        private int[] memory;
        private int numbOfPages;
        private int pageSize;
        private int maxNumbOfPages = 50;

        private int headerSize = 2;

        public Allocator(int numbOfPages, int pageSize)
        {
		if (numbOfPages< 1) {
			throw new Exception();
          }


        this.numbOfPages = numbOfPages;

        this.pageSize = pageSize;

        memory = new int[numbOfPages * pageSize + maxNumbOfPages];

		for (int i = 0; i<numbOfPages* 2; i += 2) {
        memory[i] = -((i / 2) * pageSize + maxNumbOfPages);// link to the
                                                           // page

        if (i == (numbOfPages * 2 - 2))
        {
            memory[i + 1] = 0; // no free pages after this page
        }
        else
        {
            memory[i + 1] = i + 2;// link to the header of next free page
        }
    }
  
    Console.WriteLine("Allocator successfully created!");
    }

   
     // Allocate memory block of n-bytes. If success, returns start address, otherwise -1.
    public int mem_alloc(int size)
    {
        int retAddr = -1;
        int curPage = getNeedPage(size);
        // if memory didn't have need page -> try to divide page with need
        // block's size
        if ((curPage == -1) && (size <= pageSize / 2))
        {
            curPage = dividePage(size);
            if (curPage == -1)
            {
                return -1;
            }
        }
        if ((curPage == -1) && (size > (1 / 2) * pageSize))
        {
            return -1;
        }
        // memory correction (new links to free blocks)
        if (size > pageSize / 2)
        { // size > pageSize/2
            int numbOfNeedPages = size / pageSize;
            if (size < pageSize)
            { // pageSize/2 < size < pageSize
                numbOfNeedPages = 1;
            }
            int nextFreePage = memory[curPage + 1];
            int isOnePage = -1;
            if (numbOfNeedPages > 1)
            {
                isOnePage = 1;
            }
            for (int i = curPage; i < (curPage + numbOfNeedPages * 2) - 1; i += 2)
            {
                nextFreePage = memory[i + 1];
                memory[i + 1] = isOnePage;
            }
            memory[(curPage + numbOfNeedPages * 2) - 1] = -1; // last page of
                                                              // multy-block
                                                              // of memory
            for (int i = curPage; i > 0; i -= 2)
            {
                if (memory[i + 1] == curPage)
                {
                    memory[i + 1] = nextFreePage;
                    break;
                }
            }
            retAddr = -memory[curPage];
        }
        else
        { // size <= pageSize/2
            memory[curPage + 1]--;
            retAddr = memory[memory[curPage] + 1];
            if (memory[curPage + 1] != 0)
            {
                memory[memory[curPage] + 1] = memory[retAddr];
                memory[memory[retAddr] + 1] = memory[retAddr + 1];
            }
        }
        return retAddr;
    }

       // Method search page or pages with needed block's size. Returns number of page.
        private int getNeedPage(int size)
    {
        // return memory cell with link to the need page
        if (size > pageSize / 2)
        {
            if (size <= pageSize)
            { // size < than 1 page and > 1/2 page
                for (int i = 0; i < numbOfPages * 2; i += 2)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        return i;
                    }
                }
                return -1;
            }
            else
            {
                int needNumbOfPages = size / pageSize;
                int i = 0;
                bool isEnd = false;
                while (!isEnd)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        for (int j = 1; j <= needNumbOfPages - 1; j++)
                        {
                            if (!((memory[i + j * 2] < 0) && (memory[i + 1 + j * 2] >= 0)))
                            {
                                break;
                            }
                            if (j == needNumbOfPages - 1)
                            {
                                return i;
                            }
                        }

                    }
                    i += 2;
                    if ((memory[i] == 0) && (memory[i + 1] == 0))
                    {
                        isEnd = true;
                    }
                }
            }
        }
        else
        {
            // serching needed size of block
            int needSize = pageSize / 2;
            if (size <= pageSize / 5)
            {
                needSize = pageSize / 5;
            }
            if (size <= pageSize / 10)
            {
                needSize = pageSize / 10;
            }
            if (size <= pageSize / 20)
            {
                needSize = pageSize / 20;
            }

            for (int i = 0; i < numbOfPages * 2; i += 2)
            {
                // discard not divided blocks
                if (memory[i] < 0)
                {
                    continue;
                }
                if ((memory[i + 1] > 0) // page has free blocks
                        && (memory[memory[i]] == needSize)) // block has
                                                            // need size
                {
                    return i;
                }
            }
        }
        return -1;
    }

   
	// Search whole page and divide it to blocks with need size.
    private int dividePage(int size)
    {
        // search free not divide page
        int pageLink = -1;
        for (int i = 0; i < numbOfPages * 2; i += 2)
        {
            if ((memory[i] < 0) && (memory[i + 1] >= 0))
            {
                pageLink = i;
                break;
            }
        }
        if (pageLink != -1)
        {
            // change block's size
            int needSize = pageSize / 2;
            if (size <= pageSize / 5)
            {
                needSize = pageSize / 5;
            }
            if (size <= pageSize / 10)
            {
                needSize = pageSize / 10;
            }
            if (size <= pageSize / 20)
            {
                needSize = pageSize / 20;
            }
            memory[pageLink] *= -1; // page is divides
            memory[pageLink + 1] = pageSize / needSize; // number of free blocks
                                                        // on page
                                                        // creating header of divided page
            if (needSize <= 10)
            {
                memory[pageLink + 1]--; // minus 1 block for header
                memory[memory[pageLink]] = needSize; // size of blocks on page
                memory[memory[pageLink] + 1] = needSize + maxNumbOfPages
                        + (pageLink / 2) * pageSize; // link to the first free
                                                     // block on page

                // creating linked list of free blocks of page
                int curNextLink = memory[memory[pageLink] + 1];
                int curPrewLink = -1;
                for (int i = 0; i < memory[pageLink + 1]; i++)
                {
                    if (i == memory[pageLink + 1] - 1)
                    {
                        memory[curNextLink] = -1;
                    }
                    else
                    {
                        memory[curNextLink] = curNextLink
                                + memory[memory[pageLink]];
                    }
                    memory[curNextLink + 1] = curPrewLink;
                    curPrewLink = curNextLink;
                    curNextLink = memory[curNextLink];
                }
                return pageLink;
            }
            else
            {
                memory[pageLink] = mem_alloc(headerSize);
                if (memory[pageLink] != -1)
                {
                    memory[memory[pageLink]] = needSize; // size of blocks on
                                                         // page
                    memory[memory[pageLink] + 1] = maxNumbOfPages
                            + (pageLink / 2) * pageSize; // link to the first
                                                         // free
                                                         // block on page
                                                         // creating linked list of free blocks of page
                    int curNextLink = memory[memory[pageLink] + 1];// на перший
                                                                   // вільш
                                                                   // блок
                    int curPrewLink = -1;
                    for (int i = 0; i < memory[pageLink + 1]; i++)
                    {// кількість
                     // вільних
                     // блоків
                        if (i == memory[pageLink + 1] - 1)
                        {// якщо останній
                         // блок сторінки
                            memory[curNextLink] = -1;
                        }
                        else
                        {
                            memory[curNextLink] = curNextLink
                                    + memory[memory[pageLink]];
                        }
                        memory[curNextLink + 1] = curPrewLink;
                        curPrewLink = curNextLink;
                        curNextLink = memory[curNextLink];
                    }
                    return pageLink;
                }
                else
                {
                    return -1;
                }
            }

        }
        else
        {
            return -1;
        }
    }

    // Change block size to n-size. All/partial content is moved to another block. 
    // If success, returns new addr, otherwise NULL with no changes to block.
    // If addr = -1, then call is identical to mem_alloc(size).
    public int mem_realloc(int addr, int size)
    {
        int retAddr = mem_alloc(size);
        if (retAddr != -1)
        {
            // copy data to new location
            int tsize = size;
            if (memory[memory[retAddr]] < size)
            {
                tsize = memory[memory[retAddr]];
            }
            for (int i = 0; i < tsize; i++)
            {
                memory[retAddr + i] = memory[addr + i];
            }
            mem_free(addr);
            return retAddr;
        }
        else
        {
            return -1;
        }
    }

    // Free memory area by address.
    public void mem_free(int addr)
    {

        int curPage = definePage(addr);
        if (memory[curPage] < 0)
        {
            if (memory[curPage + 1] == -1)
            { // one page
              // seach left link to the next free page
                for (int i = curPage - 2; i >= 0; i -= 2)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        memory[i + 1] = curPage; // change link to this page
                        break;
                    }
                }
                // set link to the next free block
                memory[curPage + 1] = 0;
                for (int i = curPage + 2; i < numbOfPages * 2; i += 2)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        memory[curPage + 1] = i;
                        break;
                    }
                }
            }
            else
            { // multy-block page
              // seach left link to the next free page
                for (int i = curPage - 2; i >= 0; i -= 2)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        memory[i + 1] = curPage; // change link to this page
                        break;
                    }
                }
                // set internal page links
                memory[curPage + 1] = curPage + 2;
                while (memory[curPage + 1] != -1)
                {
                    memory[curPage + 1] = curPage + 2;
                    curPage += 2;
                }
                // set link to the next free block
                memory[curPage + 1] = 0;
                for (int i = curPage + 2; i < numbOfPages * 2; i += 2)
                {
                    if ((memory[i] < 0) && (memory[i + 1] > 1))
                    {
                        memory[curPage + 1] = i;
                        break;
                    }
                }
            }

        }
        else
        { // adress is in divided page
            memory[curPage + 1]++; // add count of free blocks of page
                                   // where does heaader situate? on block (withHeader = 1) or not
                                   // (withHeader = 0)
            int withHeader = 1;
            if (memory[memory[curPage]] > 10)
            {
                withHeader = 0;
            }

            // if all blocks of page is free
            if (memory[curPage + 1] == (pageSize / memory[memory[curPage]] - withHeader))
            {
                // release page
                memory[curPage] *= -1;
                memory[curPage + 1] = -1;
                mem_free(addr);
            }
            else
            { // if need free only one block
              // if all blocks is busy
                if (memory[curPage + 1] == 1)
                {
                    memory[memory[curPage] + 1] = addr;
                    memory[addr] = -1;
                    memory[addr + 1] = -1;
                }
                else
                {
                    // first block's address > addr
                    if (memory[memory[curPage] + 1] > addr)
                    {
                        int tempAddr = memory[memory[curPage] + 1];
                        memory[memory[curPage] + 1] = addr; // addr is first
                                                            // free
                                                            // block on page
                        memory[addr] = tempAddr;
                        memory[addr + 1] = -1;
                    }
                    else
                    {
                        int curLink = memory[memory[curPage] + 1];
                        int prewLink = curLink;
                        for (int i = 0; i < memory[curPage + 1] - withHeader
                                - 1; i++)
                        {
                            if (curLink > addr)
                            {
                                int tempAddr = curLink;
                                memory[prewLink] = addr;
                                memory[addr] = tempAddr;
                                break;
                            }
                            if (i == (memory[curPage + 1] - withHeader))
                            {
                                memory[addr] = -1;
                                break;
                            }
                            prewLink = curLink;
                            curLink = memory[curLink];
                        }
                    }
                }
            }
        }
    }

    // Method defines the number of page(adress on link array) by adress of block.
    public int definePage(int addr)
    {
        return 2 * ((addr - maxNumbOfPages) / pageSize);
    }

   
    //Print state of memory areas
    public void mem_dump()
    {
        // print array of pointers to headers
        for (int i = 0; i < maxNumbOfPages; i++)
        {
                Console.Write($"{i}-page : {memory[i]}" + "\t");
                if (i % 5 == 0) Console.WriteLine();
        }
        Console.Write(Environment.NewLine);
        int point = maxNumbOfPages;
        for (int i = 0; i < numbOfPages; i++)
        {
            Console.WriteLine("Page " + i + " :");
            for (int j = 0; j < pageSize; j++)
            {
                Console.Write(memory[point] + " ");
                point++;
            }
           Console.WriteLine(Environment.NewLine);
        }
    }

        // Fill block of memory by random numbers and count xor-summ
        public void block_filling(int addr)
    {
        int curPage = definePage(addr);
        int numbOfCells = 0;
        if (memory[curPage] < 0)
        { // one or mor pages
            if (memory[curPage + 1] < 0)
            {
                numbOfCells = pageSize;
            }
            else
            {
                int numbOfPages = 1;
                int i = curPage;
                while (memory[i + 1] != -1)
                {
                    i += 2;
                }
                numbOfPages += (i - curPage) / 2;
                numbOfCells = numbOfPages * pageSize;
            }
        }
        else
        {
            numbOfCells = memory[memory[curPage]]; // size of blocks on page
        }
        int xorSumm = 0;
        for (int i = 1; i < numbOfCells; i++)
        {
            memory[i + addr] = (int)Math.Round((float)new Random().NextDouble() * 100);
            xorSumm = (xorSumm ^ memory[i + addr]); // xor-summ
        }
        memory[addr] = xorSumm;
    }

        // Check xor-summ of block.
        public bool checkSum(int addr)
    {
        int curPage = definePage(addr);
        int numbOfCells = 0;
        if (memory[curPage] < 0)
        { // one or mor pages
            if (memory[curPage + 1] < 0)
            {
                numbOfCells = pageSize;
            }
            else
            {
                int numbOfPages = 1;
                int i = curPage;
                while (memory[i + 1] != -1)
                {
                    i += 2;
                }
                numbOfPages += (i - curPage) / 2;
                numbOfCells = numbOfPages * pageSize;
            }
        }
        else
        {
            numbOfCells = memory[memory[curPage]]; // size of blocks on page
        }
        int xorSumm = 0;
        for (int i = 1; i < numbOfCells; i++)
        {
            xorSumm = (xorSumm ^ memory[i + addr]); // xor-summ
        }
        if (memory[addr] == xorSumm)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
}
