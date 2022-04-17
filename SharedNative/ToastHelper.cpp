#include "pch.h"
#include "ToastHelper.h"
#include "ToastHelper.g.cpp"

using namespace winrt::Windows::Data::Xml::Dom;
using namespace winrt::Windows::UI::Notifications;

namespace winrt::SharedNative::implementation
{
    void ToastHelper::ShowToast()
    {
        XmlDocument toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastImageAndText02);
        XmlNodeList textElements = toastXml.GetElementsByTagName(L"text");
        textElements.GetAt(0).AppendChild(toastXml.CreateTextNode(L"A toast example"));
        textElements.GetAt(1).AppendChild(toastXml.CreateTextNode(L"Hello world!"));
        ToastNotification notification{ toastXml };
        ToastNotificationManager::CreateToastNotifier().Show(notification);
    }
}
