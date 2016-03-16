__author__ = 'jiangmb'
from imposm.parser import OSMParser

# simple class that handles the parsed OSM data.
class HighwayCounter(object):
    highways = 0

    def ways(self, ways):
        # callback method for ways
        for osmid, tags, refs in ways:
            if 'highway' in tags:
              self.highways += 1

# instantiate counter and parser and start parsing
counter = HighwayCounter()
file=r"D:\opensource\imposm.parser-1.0.7.tar\dist\imposm.parser-1.0.7\imposm.parser-1.0.7\build\lib.win32-2.7\imposm\parser\test\test.pbf"
p = OSMParser(concurrency=4, ways_callback=counter.ways)
p.parse(file)

# done
print counter.highways