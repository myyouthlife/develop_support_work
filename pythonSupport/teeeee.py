
from osmread import *


highway_count = 0
for entity in parse_file(r'D:\opensource\imposm-parser\imposm\parser\test\test.pbf'):
    print entity
    if isinstance(entity, Way) and 'highway' in entity.tags:
        highway_count += 1

print "%d highways found" % highway_count