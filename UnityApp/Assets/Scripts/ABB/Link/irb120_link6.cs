/****************************************************************************
MIT License
Copyright (c) 2025 Shandon Jude Herft

Originally developed by Roman Parak (2021). Modified and extended by 
Shandon Jude Herft for the JointLab VR framework as part of a Master's thesis 
at Carnegie Mellon University.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Shandon Jude Herft
Acknowledgment: Based on original work by Roman Parak (https://github.com/rparak)
Email    : shandonherft@gmail.com
File Name: irb120_link6.cs
****************************************************************************/

// System
using System;
// Unity 
using UnityEngine;
using static abb_data_processing;
using Debug = UnityEngine.Debug;

public class irb120_link6 : MonoBehaviour
{
    void FixedUpdate()
    {
        try
        {
            transform.localEulerAngles = new Vector3((float)(-EgmCommunication.GetJointPosition(5)), 0f, 0f); // Joint 6
        }
        catch (Exception e)
        {
            Debug.Log("Exception:" + e);
        }
    }
    void OnApplicationQuit()
    {
        Destroy(this);
    }
}
