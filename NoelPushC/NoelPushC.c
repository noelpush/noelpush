// NoelPushC.cpp : définit les fonctions exportées pour l'application DLL.
//


#include "NoelPushC.h"
#include <curl/curl.h> 


unsigned char* Upload(unsigned char * data)
{
	CURL *curl = curl_easy_init();

	if (curl)
	{
		curl_easy_setopt(curl, CURLOPT_URL, "http://choco.ovh/npush/test.php");
		curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);
		curl_easy_perform(curl);
		curl_easy_cleanup(curl);
	}

	curl_global_cleanup();

	char* c = "442";
	return c;
}