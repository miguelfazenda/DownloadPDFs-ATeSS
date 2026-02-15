import sys
from dados import Dados
import dl
from downloader_excel_wintouch import DownloaderWintouch
from scraper_recibos_verdes import TipoReciboVerdePrestOUAdquir


# --- DEMO MAIN ---
def get_folder_tipo_declaracao(tipo, mes):
    if tipo == "AT_LISTA_RECIBOS_VERDES_PARA_WINTOUCH_PRESTADOS":
        return str(mes)
    return "outros"


def gen_novo_nome_ficheiro(base):
    return f"{base}_demo.txt"


class EstruturaNomesFicheiros:
    AT_LISTA_RECIBOS_VERDES_WINTOUCH_ADQUIRIDOS = "adquiridos"
    AT_LISTA_RECIBOS_VERDES_WINTOUCH_PRESTADOS = "prestados"


def demo_report_progress(progress):
    print(f"Progress: {progress}%")


if __name__ == "__main__":
    Dados.load()
    if not Dados.empresas:
        print("Nenhuma empresa carregada.")
        exit(1)
    empresa_autenticada = Dados.empresas[10]
    ano = 2025
    mes = 10
    tipo = TipoReciboVerdePrestOUAdquir.Prestador
    download_folder = "./demo_downloads"
    estrutura_nomes_ficheiros = EstruturaNomesFicheiros()

    # Define a declaration object with the correct function signature
    class DeclaracaoDemo:
        def __init__(self, tipo, download_function_mensal):
            self.download_function_mensal = download_function_mensal
            self.tipo = tipo

    # Pass a lambda that calls the correct function with driver and all args
    declaracoes = [
        DeclaracaoDemo(
            tipo=tipo,
            download_function_mensal=lambda driver, ano, mes: DownloaderWintouch.download_recibos_verdes_wintouch(
                driver,
                ano,
                mes,
                tipo,
                empresa_autenticada,
                get_folder_tipo_declaracao,
                download_folder,
                gen_novo_nome_ficheiro,
                estrutura_nomes_ficheiros,
            ),
        )
    ]
    dl.Downloader.executar(
        [empresa_autenticada],
        declaracoes,
        ano,
        mes,
        download_folder,
        report_progress=demo_report_progress,
        headless=False,
    )
    print(f"Demo export done to {download_folder}")
