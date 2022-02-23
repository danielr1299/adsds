using DataStuctures;
using Logic.InnerDataTypes;
using System;
using System.Threading;

namespace Logic
{
    public class Manager
    {
        private readonly DoubleLList<TimeData> _timeQueue;
        private readonly BST<DataX> _mainTree;
        private readonly int _maxPerBoxType;
        private readonly IUIComunicator _comunicator;
        private readonly int _maxDivides;
        private readonly Timer _deleteOldBoxesTimer;


        public Manager
            (IUIComunicator comunicator, int maxPerBoxType = 50,
            int maxDivides = 2, int dueMinutes = 2, int periodMinutes = 3)
        {
            _comunicator = comunicator;
            _mainTree = new BST<DataX>();
            _maxPerBoxType = maxPerBoxType;
            _maxDivides = maxDivides;
            TimeSpan due = new TimeSpan(0, 0, dueMinutes, 0);
            TimeSpan period = new TimeSpan(0, 0,periodMinutes, 0);
            _deleteOldBoxesTimer = new Timer(DeleteOldBoxes, null, due, period);
        }

        //10:55, 10:56, 10:57 ....
        private void DeleteOldBoxes(object state)
        {
            //check for "expired" boxes and delete the box from trees and list

        }

        public void Supply(double bottomSize, double height, int amount)
        {
            if (bottomSize > 30 || height > 30 || bottomSize <= 0 || height <= 0)
            {
                _comunicator.OnError($"Invalid box with bottomSize: {bottomSize} and height: {height}");
                return;
            }

            if (!_mainTree.Search(new DataX(bottomSize), out DataX x))
            {
                _timeQueue.AddLast(new TimeData(bottomSize,height));
                
                x = new DataX(bottomSize);
                _mainTree.Add(x);
            }

            if (!x.YTree.Search(new DataY(height), out DataY y))
            {
                //_timeQueue.AddLast(new TimeData(bottomSize, height));
                y = new DataY(height);
                x.YTree.Add(y);
            }

            y.Count += amount;

            if (y.Count > _maxPerBoxType)
            {
                _comunicator.OnError
             ($"Box type with bottomSize: {bottomSize} and height: {height} has too many boxes ({y.Count}), returning {(y.Count - _maxPerBoxType)} Boxes");
                y.Count = _maxPerBoxType;
            }

            //timeQueueList.AddFirst(XXX);
            //DataY y = new DataY();
            //y.timeListNodeRef = timeQueueList.start;
        }

        public void BoxData(double bottomSize, double height)
        {
            if (!_mainTree.Search(new DataX(bottomSize), out DataX x))
            {
                _comunicator.OnMessage($"Box with bottomSize: {bottomSize} and height: {height} not found");
            }
            else
            {
                if (!x.YTree.Search(new DataY(height), out DataY y))
                {
                    _comunicator.OnMessage($"Box with bottomSize: {bottomSize} and height: {height} not found");
                }
                else
                {
                    _comunicator.OnMessage("Data about Box:");
                    _comunicator.OnMessage($"bottomSize: {x.X}");
                    _comunicator.OnMessage($"height: {y.Y}");
                    _comunicator.OnMessage($"Count: {y.Count}");
                }
            }
        }

        public void Purchase(double bottomSize, double height, int count = 1)
        {
            double maxPercentage = 1.5;
            int divides = 0;

            while (divides < _maxDivides && _mainTree.HasRoot() && count > 0)
            {
                DataX xMatch = new DataX(-1);
                DataY yMatch = new DataY(-1);

                // Find best match in tree
                // Find best match in tree
                    _mainTree.ScanInOrder(delegate (DataX nodeX) {
                    if (nodeX.X >= bottomSize && nodeX.X <= bottomSize * maxPercentage && (xMatch.X == -1 || nodeX.X <= xMatch.X))
                    {
                        nodeX.YTree.ScanInOrder(delegate (DataY nodeY) {
                            if (nodeY.Y >= height && nodeY.Y <= height * maxPercentage && (yMatch.Y == -1 || nodeY.Y <= yMatch.Y))
                            {
                                xMatch.X = nodeX.X;
                                yMatch.Y = nodeY.Y;
                            }
                        });
                    }
                });

                // Check if no match was found
                if (xMatch.X == -1 || yMatch.Y == -1)
                {
                    _comunicator.OnError("not found any more boxes");
                    return;
                }

                // Find x node in tree
                _mainTree.Search(xMatch, out DataX x);
                x.YTree.Search(yMatch, out DataY y);
                
                int boxesTaken = Math.Min(count, y.Count);

                _comunicator.OnMessage($"Box Found: bottomSize = {x.X},height = {y.Y},Taken: {boxesTaken}");
                if (!_comunicator.OnQuestion($"Do you want {boxesTaken} boxes of bottomSize {x.X}, height =  {y.Y}"))
                {
                    return;
                }
                else
                {
                    // Decrease count value
                    y.Count -= boxesTaken;
                    count -= boxesTaken;
                    divides++;

                    // Delete the y value if the count is 0
                    if (y.Count == 0)
                    {
                        x.YTree.Remove(y);
                        _comunicator.OnMessage("It was the last box, so the box type was removed");

                        // Delete the x value if it has no nodes left
                        if (!x.YTree.HasRoot())
                        {
                            _mainTree.Remove(x);
                        }
                    }
                }
            }
        }
    }
}
