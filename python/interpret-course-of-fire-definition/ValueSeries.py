
class ValueSeries :
    """
    ValueSeries is a string formatted in one of three ways (and one deprecated ways)
    n => compiles to a single value [ n ]
    n..m => compiles to all integers between n and m, with m > n [ n, n+1, ... m-1, m ]
    n..m,s => compiles to all integers between n and m, with a step s [ n, n+s, ... m-s, m]
    n-m => compiles to all integers between n and m, with m > n [ n, n+1, ... m-1, m ] (deprecated)
    """


    def __init__(self, valueSeriesStr) :

        self._valueSeriesStr = valueSeriesStr;
        self._Parse()

    def _Parse(self) :
        start = 1
        stop = 1 
        step = 1
        values = self._valueSeriesStr

        try :
            #Check fro the deprecated format n-m
            if "-" in values and not ".." in values :
                values = values.replace( "-", ".." )

            #Check if this is a range
            if ".." in values:
                step = 1
                #Check if this has a step
                if "," in values :
                    #Yes this is lazy programming
                    step = int( values.split(",")[1] )
                    values = values.split(",")[0]
                (start, stop) = values.split("..")
                start = int(start)
                stop = int(stop)

            #Assume this is an int
            else :
                start = int(values)
                stop = start
                step = 1

        except Exception as ex:
            #TODO Log this as an error
            print(ex)
            pass

        if (start > stop and step > 0) :
            step = -1 * step

        self._startValue = start
        self._stopValue = stop
        self._stepValue = step

    def __str__(self) :
        """
        Returns a string representing the ValueSeries
        """

        if (self._startValue == self._stopValue):
            return "{}".format(self._startValue)

        if (self._stepValue == 1) :
            return "{}..{}".format(self._startValue, self._stopValue)

        return "{}..{},{}".format(self._startValue, self._stopValue, self._stepValue)

    def GetAsSet(self) :
        return (self._startValue, self._stopValue, self._stepValue)

    def GetStartValue(self) :
        return self._startValue

    def GetStopValue(self) :
        return self._stopValue

    def GetStepValue(self) :
        return self._stepValue

    def GetAsList(self, name="") :
        """
        Returns a list of strings, representing the range values.
        name is a string. If passed in the return list includes name formatted with the list
        """

        l = []

        if name == "" or name is None:
            name = "{}"

        start = self._startValue
        if self._stepValue > 0 :
            stop = self._stopValue + 1
        else :
            stop = self._stopValue - 1

        for i in range(start, stop, self._stepValue) :
            l.append(name.format(i))
        
        return l

