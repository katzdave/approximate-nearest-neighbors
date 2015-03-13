clc; clear all; close all;

A = 100;
R = 4;
iter = 10000;

s = 1:.1:10;
failper = zeros(length(s),1);
indx = 1;

for ii = s
   s2 = A/ii;
   x = rand(iter,1)*s;
   y = rand(iter,1)*s2;
   checkdim = randi(4,iter,1);
   nfail = 0;   
   for jj = 1:iter
       check = 0;
       check = check + (x(jj)+R > ii)&(checkdim(jj) == 1);
       check = check + (x(jj)-R < 0)&(checkdim(jj) == 2);
       check = check + (y(jj)+R > s2)&(checkdim(jj) == 3);
       check = check + (y(jj)-R < 0)&(checkdim(jj) == 4);
       nfail = nfail + (check > 0);
   end
   
   failper(indx) = (iter-nfail) / iter;
   indx = indx + 1;
end

plot(failper)