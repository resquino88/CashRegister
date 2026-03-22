import type { UploadInfo } from "@/endpoints";
import apiClient from "@/endpoints/apiClient";

export async function uploadFile(
  file: File,
  uploadInfo: UploadInfo,
): Promise<Blob> {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("uploadInfo.CountryId", String(uploadInfo.countryId));
  formData.append("uploadInfo.CurrencyId", String(uploadInfo.currencyId));

  const res = await apiClient.post<Blob>("/fileupload", formData, {
    responseType: "blob",
  });

  return res.data;
}
