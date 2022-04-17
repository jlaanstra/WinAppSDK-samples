#pragma once

#include "ToastHelper.g.h"

namespace winrt::SharedNative::implementation
{
    struct ToastHelper : ToastHelperT<ToastHelper>
    {
        ToastHelper() = default;

        static void ShowToast();
    };
}

namespace winrt::SharedNative::factory_implementation
{
    struct ToastHelper : ToastHelperT<ToastHelper, implementation::ToastHelper>
    {
    };
}
