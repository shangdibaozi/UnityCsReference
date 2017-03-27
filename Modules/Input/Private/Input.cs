// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine;
using UnityEngine.Scripting;

// This should exclude Linux editor and standalone but doesn't seem like the targeted _OSX and _WIN
// #defines work when generating native dependencies.

namespace UnityEngineInternal.Input
{
    public enum NativeInputEventType
    {
        DeviceConnected,
        DeviceDisconnected,

        Generic,

        KeyDown,
        KeyUp,
        KeyRepeat,

        PointerDown,
        PointerMove,
        PointerUp,
        PointerCancelled,

        Text,
        Tracking,
    }

    public struct NativeInputEvent
    {
        public NativeInputEventType type;
        public int sizeInBytes;
        public int deviceId;
        public double time; // Time on GetTimeSinceStartup() timeline in seconds. *NOT* on Time.time timeline.
    }

    // Generic.
    public struct NativeGenericEvent
    {
        public NativeInputEvent baseEvent;
        public int controlIndex;
        public int rawValue;
        public double scaledValue;
    }

    // KeyDown, KeyUp, KeyRepeat.
    public struct NativeKeyEvent
    {
        public NativeInputEvent baseEvent;
        public KeyCode key; // This is the raw key without any translation from keyboard layouts.
        public int modifiers; ////TODO: make flags field
    }

    // PointerDown, PointerMove, PointerUp.
    public struct NativePointerEvent
    {
        public NativeInputEvent baseEvent;
        public int pointerId;
        public Vector3 position;
        public Vector3 delta;
        public float pressure;
        public float rotation;
        public float tilt;
        public Vector3 radius;
        public float distance;
        public int displayIndex;
    }

    // Text.
    public struct NativeTextEvent
    {
        public NativeInputEvent baseEvent;
        public int utf32Character;
    }

    // Tracking.
    public struct NativeTrackingEvent
    {
        public NativeInputEvent baseEvent;
        public int nodeId;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    ////TODO
    public struct NativeOutputEvent
    {
    }

    // Keep in sync with InputDeviceInfo in InputDeviceData.h.
    [Serializable]
    public struct NativeInputDeviceInfo
    {
        public int deviceId;
        public string deviceDescriptor;
    }

    public enum NativeInputUpdateType
    {
        BeginFixed = 0,
        EndFixed = 1,
        BeginDynamic = 2,
        EndDynamic = 3
    }

    public delegate void NativeUpdateCallback(NativeInputUpdateType updateType);
    public delegate void NativeEventCallback(int eventCount, IntPtr eventData);
    public delegate void NativeDeviceDiscoveredCallback(NativeInputDeviceInfo deviceInfo);

    ////REVIEW: have a notification where a device can tell the HLAPI that its configuration has changed? (like e.g. the surface of a pointer has changed dimensions)

    public partial class NativeInputSystem
    {
        public static NativeUpdateCallback onUpdate;
        public static NativeEventCallback onEvents;
        public static NativeDeviceDiscoveredCallback onDeviceDiscovered;

        ////TODO: output events
        [RequiredByNativeCode]
        internal static void NotifyUpdate(NativeInputUpdateType updateType)
        {
            NativeUpdateCallback callback = onUpdate;
            if (callback != null)
                callback(updateType);
        }

        [RequiredByNativeCode]
        internal static void NotifyEvents(int eventCount, IntPtr eventData)
        {
            NativeEventCallback callback = onEvents;
            if (callback != null)
                callback(eventCount, eventData);
        }

        [RequiredByNativeCode]
        internal static void NotifyDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
        {
            NativeDeviceDiscoveredCallback callback = onDeviceDiscovered;
            if (callback != null)
                callback(deviceInfo);
        }
    }
}
