from datetime import *
import time

t1=datetime.now()

time.sleep(5)
t2=datetime.now()

t3=(t2-t1).seconds

print str(t3);